using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Services.Interfaces;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AuditReview.Query;
using ERP.BusinessModels.Enums;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class GetOrdersCountByStatusHandler : IRequestHandler<GetOrdersCountByStatusQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly SessionProvider sessionProvider;
        public GetOrdersCountByStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.authService = authService;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetOrdersCountByStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Order, bool>> predicate = null;
            predicate = x =>  x.IsActive == true && x.DealershipId != null
                && x.CreatedDate >= request.FDate.Value
                && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                && (string.IsNullOrEmpty(request.Code) || x.Id.ToString().Contains(request.Code));

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
            var order = mapper.Map<IEnumerable<GetOrder>>(entity.Item1.ToList()).ToList();

            int Created = entity.Item1.Count(item => item.OrderStatusId == (long)OrderStatusEnum.OrderInProcess);
            int AuditReviewed = entity.Item1.Count(item => item.OrderStatusId == (long)OrderStatusEnum.AccountReviewed);
            int ManagerApproved = entity.Item1.Count(item => item.OrderStatusId >= (long)OrderStatusEnum.ManagerApproved);
            int Confirmed = entity.Item1.Count(item => item.OrderStatusId >= (long)OrderStatusEnum.OrderConfirm && item.OrderStatusId <= 50);
            return new Tuple<long, long, long, long>(Created, AuditReviewed, ManagerApproved, Confirmed);
        }


    }
}

using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AuditReview.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.AuditReview.Handler
{
    public class GetOrdersOnlyByStatusHandler : IRequestHandler<GetOrdersOnlyByStatusQuery, Tuple<IEnumerable<GetOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly SessionProvider sessionProvider;
        public GetOrdersOnlyByStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.authService = authService;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetOrder>, long>> Handle(GetOrdersOnlyByStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Order, bool>> predicate = x =>
               x.IsActive == true &&
               x.DealershipId != null 
                 && x.CreatedDate >= request.FDate.Value
                && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
              && (request.StatusId == 30 ? x.OrderStatusId >= request.StatusId && x.OrderStatusId <= 50 : x.OrderStatusId == request.StatusId) &&
               (string.IsNullOrEmpty(request.Code) || x.Id.ToString().Contains(request.Code));

            Expression<Func<Entities.Models.Order, object>>[] includes = {
                x => x.Dealership,
                x => x.OrderStatus,
                x => x.CreatedBy,
                x => x.OrderProcess,
                x => x.Dealership.AccountGroup,
            };

            Expression<Func<Entities.Models.Order, object>> OrderBy = null;
            Expression<Func<Entities.Models.Order, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");
            thenInclude.Add("OrderProcess.CreatedBy");

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var order = mapper.Map<IEnumerable<GetOrder>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetOrder>, long>(order, entity.Item2);
        }
    }
}

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

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class GetOrdersByStatusHandler : IRequestHandler<GetOrdersByStatusQuery, Tuple<IEnumerable<GetOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly SessionProvider sessionProvider;
        public GetOrdersByStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.authService = authService;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetOrder>, long>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Order, bool>> predicate = null;
            predicate = x => x.IsActive == true && x.DealershipId != null && x.OrderStatusId == request.StatusId;

            Expression<Func<Entities.Models.Order, object>>[] includes = {
                x => x.Dealership,
                x => x.OrderStatus,
                x => x.OrderProcess,
                x => x.Dealership.Territory,
                x => x.Dealership.Territory.Area.Zone,
                x => x.OrderItems,
                x => x.OrderAttachments,
                x => x.CreatedBy
            };

            Expression<Func<Entities.Models.Order, object>> OrderBy = null;
            Expression<Func<Entities.Models.Order, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");
            thenInclude.Add("OrderProcess.CreatedBy");
            thenInclude.Add("OrderItems.Item");
            thenInclude.Add("OrderItems.Item.ItemType");
            thenInclude.Add("OrderItems.Item.UOM");

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, thenInclude, includes);
            var order = mapper.Map<IEnumerable<GetOrder>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetOrder>, long>(order, entity.Item2);
        }
    }
}

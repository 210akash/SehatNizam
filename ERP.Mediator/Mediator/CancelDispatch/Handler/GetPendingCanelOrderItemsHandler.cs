using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.CancelDispatch.Query;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.CancelDispatch.Handler
{
    public class GetPendingCanelOrderItemsHandler : IRequestHandler<GetPendingCancelOrderItemsQuery, List<GetOrderItems>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingCanelOrderItemsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetOrderItems>> Handle(GetPendingCancelOrderItemsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<OrderItems, object>> orderByExpression = x => x.Id;
            Expression<Func<OrderItems, object>>[] includes = {
                x => x.Item
             };

            Expression<Func<OrderItems, bool>> predicate = x => x.IsActive == true
                   && x.OrderId == request.OrderId;

            var entity = unitOfWork.Repository<OrderItems>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var OrderItems1 = entity.Item1.ToList();
            var OrderItems = mapper.Map<IEnumerable<GetOrderItems>>(OrderItems1).ToList();

            var processedDetailIds = unitOfWork.Repository<DispatchDetail>()
                .FindAll(x => x.IsActive == true && x.OrderItem.OrderId == request.OrderId && x.DispatchOrder.Dispatch.IsActive == true)
                .GroupBy(pd => pd.OrderItemId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<GetOrderItems> pendingOrders = new();

            if (request.CancelDispatchId == 0)
            {
                pendingOrders = OrderItems
                   .Where(detail =>
                   {
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                       return detail.Quantity > totalOrderedQty;
                   })
                   .ToList();

                foreach (var detail in pendingOrders)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Quantity -= Convert.ToInt32(totalOrderedQty);
                }
            }
            else
            {
                pendingOrders = OrderItems;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetOrderItems>>(pendingOrders).ToList();
            return PurchaseDemandDetail;
        }
    }
}

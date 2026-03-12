using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
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
    public class GetPendingCancelOrderHandler : IRequestHandler<GetPendingCancelOrderQuery, List<GetOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingCancelOrderHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetOrder>> Handle(GetPendingCancelOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Order, object>> orderByExpression = x => x.CreatedDate;

            var Orders = await unitOfWork.Repository<Entities.Models.Order>().GetAsync(x => x.IsActive && x.DealershipId != null
                && (x.OrderStatusId == (long)OrderStatusEnum.OrderConfirm || x.OrderStatusId == (long)OrderStatusEnum.OrderDispatched)
                , includeProperties: "OrderItems,Dealership",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var processedDetailIds = unitOfWork.Repository<DispatchDetail>()
                .GetAll(null, null, "DispatchOrder,DispatchOrder.Dispatch").Where(x => x.IsActive == true && x.DispatchOrder.Dispatch.IsActive == true)
                .GroupBy(pd => pd.OrderItemId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    DispatchQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.DispatchQty);

            var cancelDetailIds = unitOfWork.Repository<CancelDispatchDetail>()
                .GetAll(null, null, "CancelDispatch").Where(x => x.IsActive == true)
                .GroupBy(pd => pd.OrderItemId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    DispatchQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.DispatchQty);

            List<Entities.Models.Order> pendingOrder = new();

            //foreach (var item in Orders)
            //{
            //    if (item.Id != request.CancelDispatchId)
            //    {
            //        bool hasPendingDetail = false;

            //        foreach (var detail in item.OrderItems.Where(y => y.IsActive))
            //        {
            //            var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

            //            if (detail.Quantity > totalOrderedQty)
            //            {
            //                hasPendingDetail = true;
            //                break;
            //            }
            //        }

            //        if (hasPendingDetail)
            //        {
            //            pendingOrder.Add(item);
            //        }
            //    }
            //    else
            //        pendingOrder.Add(item);
            //}
            foreach (var item in Orders)
            {
                if (item.Id != request.CancelDispatchId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.OrderItems.Where(y => y.IsActive))
                    {
                        processedDetailIds.TryGetValue(detail.Id, out var processedQty);
                        cancelDetailIds.TryGetValue(detail.Id, out var canceledQty);

                        var totalHandledQty = processedQty + canceledQty;

                        // If the total handled quantity is less than ordered, it's still pending
                        if (totalHandledQty < detail.Quantity)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingOrder.Add(item);
                    }
                }
                else
                    pendingOrder.Add(item);
            }

            return mapper.Map<List<GetOrder>>(pendingOrder);
        }
    }
}

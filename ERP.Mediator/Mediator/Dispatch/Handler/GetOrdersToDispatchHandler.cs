using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetOrdersToDispatchHandler : IRequestHandler<GetOrdersToDispatchQuery, Tuple<IEnumerable<GetOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetOrdersToDispatchHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetOrder>, long>> Handle(GetOrdersToDispatchQuery request, CancellationToken cancellationToken)
        {
                Expression<Func<Entities.Models.Order, object>> orderByExpression = x => x.CreatedDate;

                var Orders = await unitOfWork.Repository<Entities.Models.Order>().GetAsync(x => x.IsActive && x.DealershipId != null
                    && x.CreatedDate >= request.FDate.Value
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                    && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                    && (request.Code == "" || x.Id.ToString() == request.Code)
                    && (x.OrderStatusId == (long)OrderStatusEnum.OrderConfirm || x.OrderStatusId == (long)OrderStatusEnum.OrderDispatched)
                    , includeProperties: "OrderItems,OrderStatus,Dealership,Dealership.Territory," +
                    "Dealership.Territory.Area.Zone",
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
                //    bool hasPendingDetail = false;

                //    foreach (var detail in item.OrderItems.Where(y => y.IsActive))
                //    {
                //        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                //        if (detail.Quantity > totalOrderedQty)
                //        {
                //            hasPendingDetail = true;
                //            break;
                //        }
                //    }

                //    if (hasPendingDetail)
                //    {
                //        pendingOrder.Add(item);
                //    }
                //}
                foreach (var item in Orders)
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

                var mappedList = mapper.Map<List<GetOrder>>(pendingOrder).ToList();
                mappedList = mappedList.Skip(request.PagingData.Skip).Take(request.PagingData.Take).ToList();
                return new Tuple<IEnumerable<GetOrder>, long>(mappedList, Orders.Count());
        }
    }
}

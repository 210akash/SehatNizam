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
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetPendingOrderHandler : IRequestHandler<GetPendingOrderQuery, List<GetOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingOrderHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetOrder>> Handle(GetPendingOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Order, object>> orderByExpression = x => x.CreatedDate;

            // Try parsing the searchParam to an int *before* the expression

            var searchParam = request.searchParam?.Trim();
            bool isIdSearch = int.TryParse(searchParam, out int parsedId);

            var Orders = await unitOfWork.Repository<Order>().GetAsync(
                filter: x => x.IsActive &&
                             x.DealershipId != null &&
                             (
                                 (isIdSearch && x.Id.ToString().Contains(searchParam)) ||
                                 (!string.IsNullOrWhiteSpace(searchParam) &&
                                  EF.Functions.Like(x.Dealership.Name, $"%{searchParam}%"))
                             ) &&
                             (x.OrderStatusId == (long)OrderStatusEnum.OrderConfirm ||
                              x.OrderStatusId == (long)OrderStatusEnum.OrderDispatched),
                includeProperties: "OrderItems,Dealership,Dealership.Territory",
                orderByDec: q => q.OrderByDescending(orderByExpression),
                take: 10
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

            foreach (var item in Orders)
            {
                if (!request.OrderId.Contains(item.Id))
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

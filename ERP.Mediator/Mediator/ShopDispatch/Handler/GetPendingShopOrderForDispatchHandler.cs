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
using ERP.Mediator.Mediator.ShopDispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.ShopDispatch.Handler
{
    public class GetPendingShopOrderForDispatchHandler : IRequestHandler<GetPendingShopOrderForDispatchQuery, List<GetShopOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingShopOrderForDispatchHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShopOrder>> Handle(GetPendingShopOrderForDispatchQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.ShopOrder, object>> orderByExpression = x => x.CreatedDate;

            // Try parsing the searchParam to an int *before* the expression

            var searchParam = request.searchParam?.Trim();
            bool isIdSearch = int.TryParse(searchParam, out int parsedId);

            var ShopOrders = await unitOfWork.Repository<Entities.Models.ShopOrder>().GetAsync(
                filter: x => x.IsActive &&
                             x.ShopId != null &&
                             (
                                 (isIdSearch && x.Id.ToString().Contains(searchParam)) ||
                                 (!string.IsNullOrWhiteSpace(searchParam) &&
                                  EF.Functions.Like(x.Shop.Name, $"%{searchParam}%"))
                             ),
                             //&&
                             //(x.ShopOrderStatusId == (long)OrderStatusEnum.OrderConfirm ||
                             // x.ShopOrderStatusId == (long)OrderStatusEnum.OrderDispatched),
                includeProperties: "ShopOrderItems,Shop,ShopOrderItems.Item",
                orderByDec: q => q.OrderByDescending(orderByExpression),
                take: 10
            );

            var processedDetailIds = unitOfWork.Repository<ShopDispatchDetail>()
                .GetAll(null, null, "ShopDispatch").Where(x => x.IsActive == true && x.ShopDispatch.IsActive == true)
                .GroupBy(pd => pd.ShopOrderItemId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    DispatchQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.DispatchQty);

            List<Entities.Models.ShopOrder> pendingShopOrder = new();

            foreach (var item in ShopOrders)
            {
                if (!request.ShopOrderId.Contains(item.Id))
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.ShopOrderItems.Where(y => y.IsActive))
                    {
                        processedDetailIds.TryGetValue(detail.Id, out var processedQty);

                        var totalHandledQty = processedQty;

                        // If the total handled quantity is less than ordered, it's still pending
                        if (totalHandledQty < detail.Quantity)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingShopOrder.Add(item);
                    }
                }
                else
                    pendingShopOrder.Add(item);
            }

            return mapper.Map<List<GetShopOrder>>(pendingShopOrder);
        }
    }
}

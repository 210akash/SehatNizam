using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using ERP.BusinessModels.Enums;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class GetPendingShopOrderHandler : IRequestHandler<GetPendingShopOrderQuery, List<GetShopOrder>>
    {
        private readonly SessionProvider sessionProvider;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingShopOrderHandler(SessionProvider sessionProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.sessionProvider = sessionProvider;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShopOrder>> Handle(GetPendingShopOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.ShopOrder, object>> orderByExpression = x => x.CreatedDate;
            var rawOrders = await unitOfWork.Repository<Entities.Models.ShopOrder>().GetAsync(
                x => x.IsActive 
                && x.ShopId == sessionProvider.Session.RetailUserShopId
                && x.ShopOrderStatusId == (long)OrderStatusEnum.OrderReceived,
                includeProperties: "ShopOrderItems",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var searchParam = request.searchParam?.Trim();
            IEnumerable<Entities.Models.ShopOrder> filteredOrders = rawOrders;
            if (!string.IsNullOrWhiteSpace(searchParam))
            {
                filteredOrders = filteredOrders.Where(x =>
                    x.Id.ToString().Contains(searchParam)); // or other fields
            }

            var Orders = filteredOrders.Take(10).ToList();
            var processedDetailIds = unitOfWork.Repository<ShopOrderReturnDetail>()
                .GetAll(null,null, "ShopOrderItems,ShopOrderItems.ShopOrder")
                .Where(x => x.IsActive && x.ShopOrderItems.ShopOrder.ShopId == sessionProvider.Session.RetailUserShopId)
                .GroupBy(pd => pd.ShopOrderReturnId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<Entities.Models.ShopOrder> pendingPurchaseOrder = new();

            foreach (var item in Orders)
            {
                if (item.Id != request.ShopOrderId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.ShopOrderItems.Where(y => y.IsActive))
                    {
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        if (detail.Quantity > totalOrderedQty)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingPurchaseOrder.Add(item);
                    }
                }
                else
                    pendingPurchaseOrder.Add(item);
            }

            return mapper.Map<List<GetShopOrder>>(pendingPurchaseOrder);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using ERP.BusinessModels.Enums;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class GetPendingRetailOrderHandler : IRequestHandler<GetPendingRetailOrderQuery, List<GetRetailOrder>>
    {
        private readonly SessionProvider sessionProvider;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingRetailOrderHandler(SessionProvider sessionProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.sessionProvider = sessionProvider;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRetailOrder>> Handle(GetPendingRetailOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.RetailOrder, object>> orderByExpression = x => x.CreatedDate;
            var rawOrders = await unitOfWork.Repository<Entities.Models.RetailOrder>().GetAsync(
                x => x.IsActive
                && (x.ShopId == sessionProvider.Session.RetailUserShopId)
                && x.RetailOrderStatusId == (long)OrderStatusEnum.OrderReceived,
                includeProperties: "RetailOrderItems",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var searchParam = request.searchParam?.Trim();
            IEnumerable<Entities.Models.RetailOrder> filteredOrders = rawOrders;
            if (!string.IsNullOrWhiteSpace(searchParam))
            {
                filteredOrders = filteredOrders.Where(x =>
                    x.Id.ToString().Contains(searchParam)); // or other fields
            }

            var Orders = filteredOrders.Take(10).ToList();
            var processedDetailIds = unitOfWork.Repository<RetailOrderReturnDetail>()
                .GetAll(null,null, "RetailOrderItems,RetailOrderItems.RetailOrder")
                .Where(x => x.IsActive && x.RetailOrderItems.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId)
                .GroupBy(pd => pd.RetailOrderReturnId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<Entities.Models.RetailOrder> pendingPurchaseOrder = new();

            foreach (var item in Orders)
            {
                if (item.Id != request.RetailOrderId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.RetailOrderItems.Where(y => y.IsActive))
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

            return mapper.Map<List<GetRetailOrder>>(pendingPurchaseOrder);
        }
    }
}

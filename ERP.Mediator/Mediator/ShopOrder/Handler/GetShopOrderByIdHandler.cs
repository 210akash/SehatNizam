using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.BusinessModels.Enums;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class GetShopOrderByIdHandler : IRequestHandler<GetShopOrderByIdQuery, GetShopOrder>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetShopOrder> Handle(GetShopOrderByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.ShopOrder, bool>> predicate = x => x.IsActive == true && x.Id == request.Id;

            Expression<Func<Entities.Models.ShopOrder, object>>[] includes = {
                x => x.ShopOrderItems.Where(x => x.IsActive),
                x => x.Shop,
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company,
                x => x.Shop.Territory.Area.Zone,
                x => x.Shop.Territory,
                x => x.ShopOrderStatus,
                x => x.Shop,
                x => x.Shop.Territory,
                x => x.Shop.RouteShop,
                x => x.Shop.Territory.Area,
                x => x.Shop.Territory.Area.Zone.Region,
                x => x.Shop.ShopRouteFrequency,
                x => x.Shop.Territory.Dealership.Where(y=>y.IsActive)
            };

            Expression<Func<Entities.Models.ShopOrder, object>> OrderBy = null;
            Expression<Func<Entities.Models.ShopOrder, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderItems.Item");
            thenInclude.Add("OrderItems.Item.UOM");
            thenInclude.Add("OrderItems.Item.ItemType");
            thenInclude.Add("Shop.RouteShop.Route");
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");
            thenInclude.Add("OrderProcess.CreatedBy");
            thenInclude.Add("Shop.ShopRouteFrequency.Route");
            thenInclude.Add("DispatchOrderDetails.Vehicle");

            var entity = unitOfWork.Repository<Entities.Models.ShopOrder>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, thenInclude, includes);
            var shopOrder = mapper.Map<IEnumerable<GetShopOrder>>(entity.Item1.ToList()).ToList().FirstOrDefault();

            //var products = shopOrder.OrderItems.Select(x => x.Item);

            //var territoryId = unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(x => x.Id == shopOrder.ShopId).Result.TerritoryId;
            //// Fetch all relevant Dealer Stocks and Shop Orders in one go
            //var dealerOrders = await unitOfWork.Repository<Entities.Models.OrderItems>().GetAsync(
            //    y => (y.Order.Dealership.TerritoryId == territoryId || y.Order.Shop.TerritoryId == territoryId) &&
            //         products.Select(p => p.Id).Contains(y.ItemId),
            //    null,
            //    null,
            //    "Order,Order.Dealership,Order.Shop"
            //);

            //if (products != null)
            //{
            //    // Group the orders by product
            //    var groupedDealerOrders = dealerOrders.GroupBy(o => o.ItemId);

            //    foreach (var item in products)
            //    {
            //        // Get all orders for this product
            //        var ordersForProduct = groupedDealerOrders.FirstOrDefault(g => g.Key == item.Id);
            //        if (ordersForProduct != null)
            //        {
            //            // Dealer Orders
            //            var dealerStock = ordersForProduct.Where(y =>
            //                y.Order.Dealership != null &&
            //                y.Order.Dealership.TerritoryId == territoryId &&
            //                y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived
            //            );

            //            // Total Dealer Stock Quantity
            //            int totalDealerQty = dealerStock.Sum(s => s.Quantity);

            //            // Shop Orders
            //            var shopOrders = ordersForProduct.Where(y =>
            //                y.Order.Shop != null &&
            //                y.Order.Shop.TerritoryId == territoryId
            //            );

            //            // Total Shop Sold Quantity
            //            int totalDealerSoldQty = shopOrders
            //                .Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderDispatched)
            //                .Sum(s => s.Quantity);

            //            // Left Quantity
            //            item.LeftQuantity = totalDealerQty - totalDealerSoldQty;

            //            // Total Shop Requested Quantity (orders not received yet)
            //            int totalShopReqQty = shopOrders
            //                .Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderCreate || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderInProcess
            //                || y.Order.OrderStatusId == (long)OrderStatusEnum.AccountReviewed || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderConfirm)
            //                .Sum(s => s.Quantity);

            //            // Hold Quantity
            //            item.HoldQuantity = totalShopReqQty;
            //        }
            //    }
            //}

            //foreach (var orderItem in shopOrder.OrderItems)
            //{
            //    // Get the updated product and map the values of LeftQuantity and HoldQuantity
            //    var updatedProduct = products.FirstOrDefault(p => p.Id == orderItem.ItemId);
            //    if (updatedProduct != null)
            //    {
            //        orderItem.LeftQuantity = updatedProduct.LeftQuantity;
            //        orderItem.HoldQuantity = updatedProduct.HoldQuantity;
            //    }
            //}

            return shopOrder;
        }
    }
}

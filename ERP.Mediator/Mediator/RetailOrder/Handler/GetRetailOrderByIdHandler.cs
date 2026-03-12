using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class GetRetailOrderByIdHandler : IRequestHandler<GetRetailOrderByIdQuery, GetRetailOrder>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetRetailOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<GetRetailOrder> Handle(GetRetailOrderByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.RetailOrder, bool>> predicate = x => x.IsActive == true && x.Id == request.Id;

            Expression<Func<Entities.Models.RetailOrder, object>>[] includes = {
                x => x.RetailOrderItems.Where(x => x.IsActive),
                x => x.Shop,
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company,
                x => x.Shop.Territory.Area.Zone,
                x => x.Shop.Territory,
                x => x.RetailOrderStatus,
                x => x.Shop.RouteShop,
                x => x.RetailOrderProcess,
                x => x.Shop.Territory.Area,
                x => x.Shop.Territory.Area.Zone.Region,
                x => x.Shop.ShopRouteFrequency,
                x => x.Shop.Territory.Dealership,
            };

            Expression<Func<Entities.Models.RetailOrder, object>> OrderBy = null;
            Expression<Func<Entities.Models.RetailOrder, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("RetailOrderItems.Item");
            thenInclude.Add("RetailOrderItems.Item.UOM");
            thenInclude.Add("RetailOrderItems.Item.ItemType");
            thenInclude.Add("Shop.RouteShop.Route");
            thenInclude.Add("RetailOrderProcess.FromStatus");
            thenInclude.Add("RetailOrderProcess.ToStatus");
            thenInclude.Add("RetailOrderProcess.CreatedBy");
            thenInclude.Add("Shop.ShopRouteFrequency.Route");

            var entity = unitOfWork.Repository<Entities.Models.RetailOrder>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, thenInclude, includes);
            var RetailOrder = mapper.Map<IEnumerable<GetRetailOrder>>(entity.Item1.ToList()).ToList().FirstOrDefault();

            var products = RetailOrder.RetailOrderItems.Select(x => x.Item);

            //var territoryId = unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(x => x.Id == RetailOrder.ShopId).Result.TerritoryId;
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
            //            var RetailOrders = ordersForProduct.Where(y =>
            //                y.Order.Shop != null &&
            //                y.Order.Shop.TerritoryId == territoryId
            //            );

            //            // Total Shop Sold Quantity
            //            int totalDealerSoldQty = RetailOrders
            //                .Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderDispatched)
            //                .Sum(s => s.Quantity);

            //            // Left Quantity
            //            item.LeftQuantity = totalDealerQty - totalDealerSoldQty;

            //            // Total Shop Requested Quantity (orders not received yet)
            //            int totalShopReqQty = RetailOrders
            //                .Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderCreate || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderInProcess
            //                || y.Order.OrderStatusId == (long)OrderStatusEnum.AccountReviewed || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderConfirm)
            //                .Sum(s => s.Quantity);

            //            // Hold Quantity
            //            item.HoldQuantity = totalShopReqQty;
            //        }
            //    }
            //}

            if (products != null)
            {
                // Fetch all relevant Dealer Stocks and Shop Orders in one go
                var _shopOrders = await unitOfWork.Repository<ShopDispatchDetail>().GetAsync(
                    y => y.ShopOrderItem.ShopOrder.ShopId == sessionProvider.Session.RetailUserShopId && y.IsActive == true && y.ShopOrderItem.IsActive == true && y.IsDelete == false && y.ShopDispatch.IsActive && y.ShopDispatch.StatusId == (long)OrderStatusEnum.OrderReceived &&
                         products.Select(p => p.Id).Contains(y.ShopOrderItem.ItemId),
                    null,
                    null,
                    "ShopOrderItem,ShopOrderItem.ShopOrder,ShopOrderItem.ShopOrder.Shop,ShopDispatch"
                );

                var _retailOrders = await unitOfWork.Repository<RetailOrderItems>().GetAsync(
                    y => y.IsActive &&
                         !y.IsDelete &&
                         y.RetailOrder.IsActive &&
                         y.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId &&
                         //y.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.OrderReceived &&
                         products.Select(p => p.Id).Contains(y.ItemId),
                    null,
                    null,
                    "RetailOrder,RetailOrder.Shop"
                );

                var _returnOrders = await unitOfWork.Repository<RetailOrderReturnDetail>().GetAsync(
                 y => y.IsActive &&
                      !y.IsDelete &&
                      y.RetailOrderReturn.StatusId == 3 &&
                      y.RetailOrderItems.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId &&
                      y.RetailOrderItems.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.OrderReceived &&
                      products.Select(p => p.Id).Contains(y.RetailOrderItems.ItemId),
                 null,
                 null,
                 "RetailOrderReturn,RetailOrderItems,RetailOrderItems.RetailOrder"
             );

                // Group the orders by product
                var groupedshopOrders = _shopOrders.GroupBy(o => o.ShopOrderItem.ItemId);
                var groupedretailOrders = _retailOrders.GroupBy(o => o.ItemId);
                var groupedreturnOrders = _returnOrders.GroupBy(o => o.RetailOrderItems.ItemId);

                foreach (var item in products)
                {
                    // Get all orders for this product
                    var ordersForProduct = groupedshopOrders.FirstOrDefault(g => g.Key == item.Id);
                    var RetailordersForProduct = groupedretailOrders.FirstOrDefault(g => g.Key == item.Id);
                    var ReturnordersForProduct = groupedreturnOrders.FirstOrDefault(g => g.Key == item.Id);

                    if (ordersForProduct != null)
                    {
                        // Shop Orders
                        var totalShopQty = (int)ordersForProduct.Where(y =>
                            y.ShopOrderItem.ItemId == item.Id
                        ).Sum(s => s.Quantity);

                        // Retail Orders
                        var retailOrders = RetailordersForProduct.Where(y =>
                           y.ItemId == item.Id
                        );

                        // Total Shop Sold Quantity
                        int totalRetailSoldQty = retailOrders
                            .Where(y => y.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.OrderReceived)
                            .Sum(s => s.Quantity);

                        int returnOrders = 0;
                        if (ReturnordersForProduct != null)
                        {
                            // Return Orders
                            returnOrders = (int)ReturnordersForProduct.Where(y =>
                               y.RetailOrderItems.ItemId == item.Id
                            ).Sum(y => y.Quantity);

                        }

                        // Left Quantity
                        item.LeftQuantity = totalShopQty + returnOrders - totalRetailSoldQty;
                        Console.WriteLine("item : " + item.Name, ", Total In : " + totalShopQty + ", Sold : " + totalRetailSoldQty);
                        System.Diagnostics.Debug.WriteLine($"item: {item.Name}, Total In: {totalShopQty}, Sold: {totalRetailSoldQty}");

                        // Total Shop Requested Quantity (orders not received yet)
                        int totalShopReqQty = retailOrders
                            .Where(y => y.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.OrderCreate || y.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.OrderInProcess
                            || y.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.AccountReviewed || y.RetailOrder.RetailOrderStatusId == (long)OrderStatusEnum.OrderConfirm)
                            .Sum(s => s.Quantity);

                        // Hold Quantity
                        item.HoldQuantity = totalShopReqQty;

                        var orderItem = RetailOrder.RetailOrderItems.FirstOrDefault(o => o.ItemId == item.Id);
                        if (orderItem != null)
                        {
                            orderItem.LeftQuantity = item.LeftQuantity;
                            orderItem.HoldQuantity = item.HoldQuantity;
                        }
                    }
                }
            }

            //foreach (var orderItem in RetailOrder.RetailOrderItems)
            //{
            //    // Get the updated product and map the values of LeftQuantity and HoldQuantity
            //    var updatedProduct = products.FirstOrDefault(p => p.Id == orderItem.ItemId);
            //    if (updatedProduct != null)
            //    {
            //        orderItem.LeftQuantity = updatedProduct.LeftQuantity;
            //        orderItem.HoldQuantity = updatedProduct.HoldQuantity;
            //    }
            //}

            return RetailOrder;
        }
    }
}

using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class GetKCItemsByDistributorRetailHandler : IRequestHandler<GetKCItemsByDistributorRetailQuery, List<GetItemStock>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;

        public GetKCItemsByDistributorRetailHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider, IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<List<GetItemStock>> Handle(GetKCItemsByDistributorRetailQuery request, CancellationToken cancellationToken)
        {
            var products = (from item in unitOfWork.Repository<Entities.Models.Item>().GetAll()
                            join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
                                on item.ItemTypeId equals itemType.Id
                            join subCategory in unitOfWork.Repository<Entities.Models.SubCategory>().GetAll()
                                on itemType.SubCategoryId equals subCategory.Id
                            join category in unitOfWork.Repository<Entities.Models.Category>().GetAll()
                                on subCategory.CategoryId equals category.Id
                            join categoryStore in unitOfWork.Repository<CategoryStore>().GetAll()
                                on category.Id equals categoryStore.CategoryId
                            join store in unitOfWork.Repository<Entities.Models.Store>().GetAll()
                                on categoryStore.StoreId equals store.Id
                            where store.Id == 3 && category.CompanyId == 2
                            orderby item.Name
                            select new GetItemStock
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Type = itemType.Name,
                                Volume = item.Volume,
                                QuantityInPack = item.QuantityInPack,
                                Image = item.Image,
                                IsActive = item.IsActive
                            }).ToList();

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
                    }
                }
            }

            var distributorPriceGroups = await unitOfWork.Repository<DistributorPriceGroup>().GetAsync(x => x.DealershipId == sessionProvider.Session.DealershipId && x.IsActive == true && x.IsDelete == false);
            if (distributorPriceGroups == null || distributorPriceGroups.Count() == 0)
            {
                throw new InvalidOperationException("No active Distributor Price Groups found for the Selected Distributor");
            }

            var priceGroupIds = distributorPriceGroups.Select(x => x.PriceGroupId).ToList();

            var productIds = products.Select(p => p.Id).ToList();
            var priceDetails = await unitOfWork.Repository<PriceGroupDetails>().GetAsync(x => priceGroupIds.Contains(x.PriceGroupId) && productIds.Contains(x.ItemId) && x.IsActive == true && x.IsDelete == false);

            foreach (var productDto in products)
            {
                var priceDetail = priceDetails.FirstOrDefault(p => p.ItemId == productDto.Id);
                if (priceDetail != null)
                {
                    productDto.RetailPrice = (int)priceDetail.RetailPrice;
                    productDto.TradePrice = (int)priceDetail.TradePrice;
                    productDto.DistributorPrice = (int)priceDetail.NetDistributorPrice;
                    productDto.DistributorPromo = (int)priceDetail.DistributorPromo;
                }
            }

            return products;
        }
    }
}

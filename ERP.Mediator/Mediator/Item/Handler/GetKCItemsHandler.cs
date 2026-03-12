using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Command;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Stripe;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetKCItemsHandler : IRequestHandler<GetKCItemsQuery, List<GetItem>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;

        public GetKCItemsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider, IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<List<GetItem>> Handle(GetKCItemsQuery request, CancellationToken cancellationToken)
        {

            var reportQuery = "EXEC GetStockTransaction";
            var stockTransactions = unitOfWorkDapper.Repository<StockTransactionDTO>()
                .QueryAsync<StockTransactionDTO>(reportQuery)
                .GetAwaiter().GetResult();

            var customOrder = new List<long> { 8, 10, 9, 12, 5, 7, 6, 13, 14, 15, 16, 11, 2, 4, 3 };

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
                            orderby customOrder.IndexOf(item.Id)
                            select item).ToList();

            // Fetch all relevant Dealer Stocks and Shop Orders in one go
            var dealerOrders = await unitOfWork.Repository<OrderItems>().GetAsync(
                y => (y.Order.Dealership.TerritoryId == request.TerritoryId || y.Order.Shop.TerritoryId == request.TerritoryId) &&
                     products.Select(p => p.Id).Contains(y.ItemId),
                null,
                null,
                "Order,Order.Dealership,Order.Shop"
            );

            var distributorPriceGroups = await unitOfWork.Repository<DistributorPriceGroup>().GetAsync(x => x.Dealership.TerritoryId == request.TerritoryId && x.IsActive == true && x.IsDelete == false);
            if (distributorPriceGroups == null || distributorPriceGroups.Count() == 0)
            {
                throw new InvalidOperationException("No active Distributor Price Groups found for the Selected Distributor");
            }

            var priceGroupIds = distributorPriceGroups.Select(x => x.PriceGroupId).ToList();

            var productIds = products.Select(p => p.Id).ToList();
            var priceDetails = await unitOfWork.Repository<PriceGroupDetails>().GetAsync(x => priceGroupIds.Contains(x.PriceGroupId) && productIds.Contains(x.ItemId) && x.IsActive == true && x.IsDelete == false);

            var productDtos = mapper.Map<List<GetItem>>(products);

            foreach (var productDto in productDtos)
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

            if (productDtos != null)
            {
                // Group the orders by product
                var groupedDealerOrders = dealerOrders.GroupBy(o => o.ItemId);

                foreach (var item in productDtos)
                {
                   
                    // Get all orders for this product
                    var ordersForProduct = groupedDealerOrders.FirstOrDefault(g => g.Key == item.Id);
                    if (ordersForProduct != null)
                    {
                        // Dealer Orders
                        var dealerStock = ordersForProduct.Where(y =>
                            y.Order.Dealership != null &&
                            y.Order.Dealership.TerritoryId == request.TerritoryId &&
                            y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived
                        );

                        // Total Dealer Stock Quantity
                        int totalDealerQty = dealerStock.Sum(s => s.ShippedQuantity ?? 0);

                        // Shop Orders
                        var shopOrders = ordersForProduct.Where(y =>
                            y.Order.Shop != null &&
                            y.Order.Shop.TerritoryId == request.TerritoryId
                        );

                        // Total Shop Sold Quantity
                        int totalDealerSoldQty = shopOrders
                            .Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderDispatched)
                            .Sum(s => s.ShippedQuantity ?? 0);

                        // Left Quantity
                        //item.LeftQuantity = totalDealerQty - totalDealerSoldQty;

                        // Total Shop Requested Quantity (orders not received yet)
                        int totalShopReqQty = shopOrders
                            .Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderCreate || y.Order.OrderStatusId == (long)OrderStatusEnum.OrderConfirm)
                            .Sum(s => s.Quantity);

                        // Hold Quantity
                        item.HoldQuantity = totalShopReqQty;

                        var matchingStock = stockTransactions.FirstOrDefault(s => s.ItemId == item.Id);
                        if (matchingStock != null)
                        {
                            item.LeftQuantity = (int)matchingStock.StockQty;
                        }
                        else
                            item.LeftQuantity = 0;

                    }
                }
            }

            return productDtos;
        }

    }
}

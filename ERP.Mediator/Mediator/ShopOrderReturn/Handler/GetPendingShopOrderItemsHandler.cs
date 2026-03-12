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

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class GetPendingShopOrderItemsHandler : IRequestHandler<GetPendingShopOrderItemsQuery, List<GetShopOrderItems>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingShopOrderItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShopOrderItems>> Handle(GetPendingShopOrderItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<ShopOrderItems, object>> orderByExpression = x => x.Id;
            Expression<Func<ShopOrderItems, object>>[] includes = {
                x => x.Item,
                x => x.ShopOrder,
             };

            Expression<Func<ShopOrderItems, bool>> predicate = x => x.IsActive == true
                   && x.ShopOrderId == request.ShopOrderId;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships
            var entity = unitOfWork.Repository<ShopOrderItems>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var PurchaseDemands = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<ShopOrderReturnDetail>()
                .FindAll(x => x.IsActive == true && x.ShopOrderItems.ShopOrderId == request.ShopOrderId)
                .GroupBy(pd => pd.ShopOrderItemsId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<ShopOrderItems> pendingPurchaseDemands = new();

            if (request.ShopOrderReturnId == 0)
            {
                // Filter PurchaseDemandDetails that still have pending quantities
                pendingPurchaseDemands = PurchaseDemands
                   .Where(detail =>
                   {
                       // Check if there is a processed quantity for this detail, otherwise assume 0
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                       // Check if the required quantity is greater than the total Ordered quantity (pending)
                       return detail.Quantity > totalOrderedQty;
                   })
                   .ToList();  // Ensure you get the filtered list

                // Update the Required quantity by subtracting the OrderedQty
                foreach (var detail in pendingPurchaseDemands)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Quantity -= (int)totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                pendingPurchaseDemands = PurchaseDemands;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetShopOrderItems>>(pendingPurchaseDemands).ToList();
            return PurchaseDemandDetail;
        }
    }
}

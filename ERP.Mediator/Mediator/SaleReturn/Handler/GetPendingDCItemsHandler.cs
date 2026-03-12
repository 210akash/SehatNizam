using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class GetPendingDCItemsHandler : IRequestHandler<GetPendingDCItemsQuery, List<GetDispatchDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingDCItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDispatchDetail>> Handle(GetPendingDCItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<DispatchDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<DispatchDetail, object>>[] includes = {
                x => x.OrderItem,
                x => x.OrderItem.Item,
                //x => x.PurchaseDemandDetail,
                //x => x.PurchaseDemandDetail.Item
             };

            Expression<Func<DispatchDetail, bool>> predicate = x => x.IsActive == true
                   && x.DispatchOrderId == request.DispatchOrderId;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships
            var entity = unitOfWork.Repository<DispatchDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var PurchaseDemands = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<SaleReturnDetail>()
                .FindAll(x => x.IsActive == true && x.DispatchDetail.DispatchOrderId == request.DispatchOrderId)
                .GroupBy(pd => pd.DispatchDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<DispatchDetail> pendingPurchaseDemands = new();

            if (request.SaleReturnId == 0)
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
                    detail.Quantity -= (long)totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                pendingPurchaseDemands = PurchaseDemands;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetDispatchDetail>>(pendingPurchaseDemands).ToList();
            return PurchaseDemandDetail;
        }
    }
}

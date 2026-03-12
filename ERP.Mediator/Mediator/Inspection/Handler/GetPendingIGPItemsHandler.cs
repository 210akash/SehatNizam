using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class GetPendingIGPItemsHandler : IRequestHandler<GetPendingIGPItemsQuery, List<GetIGPDetails>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingIGPItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetIGPDetails>> Handle(GetPendingIGPItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<IGPDetails, object>> orderByExpression = x => x.Id;
            Expression<Func<IGPDetails, object>>[] includes = {
                x => x.IGP,
                x => x.PurchaseOrderDetail,
                x => x.PurchaseOrderDetail.PurchaseDemandDetail,
                x => x.PurchaseOrderDetail.PurchaseDemandDetail.Item,
                x => x.PurchaseOrderDetail.PurchaseDemandDetail.Item.UOM
             };

            Expression<Func<IGPDetails, bool>> predicate = x => x.IsActive == true
                   && x.IGPId == request.IGPId;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships
            var entity = unitOfWork.Repository<IGPDetails>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var PurchaseDemands = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<InspectionDetail>()
                   .FindAll(x => x.IsActive == true && x.IGPDetail.IGPId == request.IGPId)
                   .GroupBy(pd => pd.IGPDetailId)
                   .Select(g => new { DetailId = g.Key, TotalRejectedQty = g.Sum(pd => pd.Rejected) })
                   .ToDictionary(x => x.DetailId, x => x.TotalRejectedQty);

            List<IGPDetails> pendingPurchaseDemands = new();

            if (request.InspectionId == 0)
            {
                // Filter PurchaseDemandDetails that still have pending quantities
                pendingPurchaseDemands = PurchaseDemands
                  .Where(detail =>
                  {
                      // Fetch total rejected quantity, default to 0 if not found
                      var totalRejectedQty = processedDetailIds.TryGetValue(detail.Id, out var rejectedQty) ? rejectedQty : 0;

                      // Calculate approved quantity (Received - Rejected)
                      var approvedQty = detail.Received - totalRejectedQty;

                      // Check if approved quantity is still pending (not yet fully processed)
                      return approvedQty > 0;
                  })
                   .ToList();  // Ensure you get the filtered list

                // Compute pending quantity (Approved Qty) separately
                foreach (var detail in pendingPurchaseDemands)
                {
                    var totalRejectedQty = processedDetailIds.TryGetValue(detail.Id, out var rejectedQty) ? rejectedQty : 0;

                    // Approved Quantity = Received - Rejected
                    detail.Received = detail.Received - totalRejectedQty;  // ✅ Store in a separate field
                }
            }
            else
            {
                pendingPurchaseDemands = PurchaseDemands;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetIGPDetails>>(pendingPurchaseDemands).ToList();
            return PurchaseDemandDetail;
        }
    }
}

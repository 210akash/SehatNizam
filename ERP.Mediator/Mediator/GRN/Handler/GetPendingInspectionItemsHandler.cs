using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetPendingInspectionItemsHandler : IRequestHandler<GetPendingInspectionsItemsQuery, List<GetInspectionDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingInspectionItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetInspectionDetail>> Handle(GetPendingInspectionsItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<InspectionDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<InspectionDetail, object>>[] includes = {
                x => x.Inspection,
                x => x.IGPDetail,
                x => x.IGPDetail.PurchaseOrderDetail,
                x => x.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail,
                x => x.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item,
                x => x.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType,
                x => x.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory,
                x => x.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category,
                x => x.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category.CategoryStores,
             };

            Expression<Func<InspectionDetail, bool>> predicate = x => x.IsActive == true
                   && x.InspectionId == request.InspectionId;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships
            var entity = unitOfWork.Repository<InspectionDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var PurchaseDemands1 = entity.Item1.ToList();
            var PurchaseDemands = mapper.Map<IEnumerable<GetInspectionDetail>>(PurchaseDemands1).ToList();

            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<GRNDetail>()
                .FindAll(x => x.IsActive == true && x.InspectionDetail.InspectionId == request.InspectionId)
                .GroupBy(pd => pd.InspectionDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Received) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<GetInspectionDetail> pendingPurchaseDemands = new();

            if (request.GRNId == 0)
            {
                // Filter PurchaseDemandDetails that still have pending quantities
                pendingPurchaseDemands = PurchaseDemands
                   .Where(detail =>
                   {
                       // Check if there is a processed quantity for this detail, otherwise assume 0
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                       // Check if the required quantity is greater than the total Ordered quantity (pending)
                       return  detail.Approved > totalOrderedQty;
                   })
                   .ToList();  // Ensure you get the filtered list

                //Update the Required quantity by subtracting the OrderedQty
                foreach (var detail in pendingPurchaseDemands)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Approved -= totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                //// Get current GRN details to include their quantities back
                //var currentGrnDetails = unitOfWork.Repository<GRNDetail>()
                //    .FindAll(x => x.IsActive && x.GRNId == request.GRNId)
                //    .ToList();

                //// Create a dictionary of current GRN InspectionDetailId -> quantity
                //var currentGrnQuantities = currentGrnDetails
                //    .GroupBy(x => x.InspectionDetailId)
                //    .ToDictionary(g => g.Key, g => g.Sum(x => x.Received));

                //pendingPurchaseDemands = PurchaseDemands
                //    .Where(detail =>
                //    {
                //        var alreadyReceivedQty = processedDetailIds.TryGetValue(detail.Id, out var totalReceived) ? totalReceived : 0;
                //        var currentGrnQty = currentGrnQuantities.TryGetValue(detail.Id, out var qtyInCurrentGrn) ? qtyInCurrentGrn : 0;

                //        // Calculate the effective received quantity excluding the current GRN
                //        var effectiveReceivedQty = alreadyReceivedQty - currentGrnQty;

                //        return detail.Approved > effectiveReceivedQty;
                //    })
                //    .ToList();

                //// Update Approved by subtracting effectiveReceivedQty
                //foreach (var detail in pendingPurchaseDemands)
                //{
                //    var alreadyReceivedQty = processedDetailIds.TryGetValue(detail.Id, out var totalReceived) ? totalReceived : 0;
                //    var currentGrnQty = currentGrnQuantities.TryGetValue(detail.Id, out var qtyInCurrentGrn) ? qtyInCurrentGrn : 0;

                //    var effectiveReceivedQty = alreadyReceivedQty - currentGrnQty;
                //    detail.Approved -= effectiveReceivedQty;
                //}
                pendingPurchaseDemands = PurchaseDemands;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetInspectionDetail>>(pendingPurchaseDemands).ToList();
            return PurchaseDemandDetail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class GetPendingPOItemsHandler : IRequestHandler<GetPendingPOItemsQuery, List<GetPurchaseOrderDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingPOItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPurchaseOrderDetail>> Handle1(GetPendingPOItemsQuery request, CancellationToken cancellationToken)
        {
            var purchaseOrderIds = unitOfWork.Repository<PurchaseOrderDetail>().GetAsync(x => x.PurchaseOrderId == request.PurchaseOrderId).Result.Select(x => x.PurchaseDemandDetailId);

            Expression<Func<PurchaseOrderDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<PurchaseOrderDetail, object>>[] includes = {
                x => x.PurchaseOrder,
                x => x.PurchaseDemandDetail,
                x => x.PurchaseDemandDetail.Item
             };

            Expression<Func<PurchaseOrderDetail, bool>> predicate = x => x.IsActive == true
            && x.PurchaseOrderId == request.PurchaseOrderId
            && purchaseOrderIds.Contains(x.PurchaseDemandDetailId);

            var entity = unitOfWork.Repository<PurchaseOrderDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var purchaseOrderDetails = entity.Item1.ToList();

            var processedDetailIds = unitOfWork.Repository<IGPDetails>()
                .FindAll(x => x.IsActive == true && purchaseOrderIds.Contains(x.PurchaseOrderDetail.Id))
                .GroupBy(pd => pd.PurchaseOrderDetailId)
                .Select(g => new { DetailId = g.Key, TotalReceivedQty = g.Sum(pd => pd.Received) })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<PurchaseOrderDetail> pendingPurchaseOrderDetail = new();

            if (request.PurchaseOrderId == 0)
            {
                pendingPurchaseOrderDetail = purchaseOrderDetails
                   .Where(detail =>
                   {
                       var totalReceivedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                       return detail.Quantity > totalReceivedQty;
                   })
                   .ToList();
            }
            else
            {
                pendingPurchaseOrderDetail = purchaseOrderDetails;
            }

            foreach (var detail in pendingPurchaseOrderDetail)
            {
                var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                detail.Quantity -= totalOrderedQty;
            }

            var PurchaseOrderDetail = mapper.Map<IEnumerable<GetPurchaseOrderDetail>>(pendingPurchaseOrderDetail).ToList();
            return PurchaseOrderDetail;
        }

        public async Task<List<GetPurchaseOrderDetail>> Handle(GetPendingPOItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<PurchaseOrderDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<PurchaseOrderDetail, object>>[] includes = {
                x => x.PurchaseOrder,
                x => x.PurchaseDemandDetail,
                x => x.PurchaseDemandDetail.Item
             };

            Expression<Func<PurchaseOrderDetail, bool>> predicate = x => x.IsActive == true
                   && x.PurchaseOrderId == request.PurchaseOrderId;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships
            var entity = unitOfWork.Repository<PurchaseOrderDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var PurchaseDemands = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<IGPDetails>()
                .FindAll(x => x.IsActive == true && x.PurchaseOrderDetail.PurchaseOrderId == request.PurchaseOrderId)
                .GroupBy(pd => pd.PurchaseOrderDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Received) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<PurchaseOrderDetail> pendingPurchaseDemands = new();

            if (request.IGPId == 0)
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
                    detail.Quantity -= totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                pendingPurchaseDemands = PurchaseDemands;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetPurchaseOrderDetail>>(pendingPurchaseDemands).ToList();
            return PurchaseDemandDetail;
        }
    }
}

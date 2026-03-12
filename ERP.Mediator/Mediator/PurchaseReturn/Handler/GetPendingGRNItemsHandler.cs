using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class GetPendingGRNItemsHandler : IRequestHandler<GetPendingGRNItemsQuery, List<GetGRNDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingGRNItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetGRNDetail>> Handle(GetPendingGRNItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<GRNDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<GRNDetail, object>>[] includes = {
                x => x.InspectionDetail,
                x => x.InspectionDetail.IGPDetail,
                x => x.InspectionDetail.IGPDetail.PurchaseOrderDetail,
                x => x.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder,
                x => x.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.Vendor,
                x => x.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Project,
                x => x.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item
             };

            Expression<Func<GRNDetail, bool>> predicate = x => x.IsActive == true
                   && x.GRNId == request.GRNId;

            // Fetch all active GRN for the current store, including necessary relationships
            var entity = unitOfWork.Repository<GRNDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var GRN = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<PurchaseReturnDetail>()
                .FindAll(x => x.IsActive == true && x.GRNDetail.GRNId == request.GRNId)
                .GroupBy(pd => pd.GRNDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<GRNDetail> pendingGRN = new();

            if (request.PurchaseReturnId == 0)
            {
                // Filter PurchaseDemandDetails that still have pending quantities
                pendingGRN = GRN
                   .Where(detail =>
                   {
                       // Check if there is a processed quantity for this detail, otherwise assume 0
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                       // Check if the required quantity is greater than the total Ordered quantity (pending)
                       return detail.Received > totalOrderedQty;
                   })
                   .ToList();  // Ensure you get the filtered list

                // Update the Required quantity by subtracting the OrderedQty
                foreach (var detail in pendingGRN)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Received -= (long)totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                pendingGRN = GRN;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetGRNDetail>>(pendingGRN).ToList();
            return PurchaseDemandDetail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class GetPendingDemandItemsHandler : IRequestHandler<GetPendingDemandItemsQuery, List<GetPurchaseDemandDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingDemandItemsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetPurchaseDemandDetail>> Handle(GetPendingDemandItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<PurchaseDemandDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<PurchaseDemandDetail, object>> includeProperties = x => x.Item.UOM;

            Expression<Func<PurchaseDemandDetail, object>>[] includes = {
                x => x.Item,
                x => x.Item.UOM,
                x => x.Department,
                x => x.Project,
                x => x.ComparativeStatementDetail.ComparativeStatementVendor.Where(y=>y.VendorId == request.VendorId),
             };

            Expression<Func<PurchaseDemandDetail, bool>> predicate = x => x.IsActive == true
                   && x.PurchaseDemandId == request.PurchaseDemandId;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships
            var entity = unitOfWork.Repository<PurchaseDemandDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var PurchaseDemands = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds =  unitOfWork.Repository<PurchaseOrderDetail>()
                .FindAll(x=> x.IsActive == true  && x.PurchaseDemandDetail.PurchaseDemandId == request.PurchaseDemandId)
                .GroupBy(pd => pd.PurchaseDemandDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<PurchaseDemandDetail> pendingPurchaseDemands = new();

            if(request.PurchaseOrderId == 0)
            {
                // Filter PurchaseDemandDetails that still have pending quantities
                 pendingPurchaseDemands = PurchaseDemands
                    .Where(detail =>
                    {
                        // Check if there is a processed quantity for this detail, otherwise assume 0
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        // Check if the required quantity is greater than the total Ordered quantity (pending)
                        return detail.DemandQty > totalOrderedQty;
                    })
                    .ToList();  // Ensure you get the filtered list

                // Update the Required quantity by subtracting the OrderedQty
                foreach (var detail in pendingPurchaseDemands)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.DemandQty -= totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                pendingPurchaseDemands = PurchaseDemands;  
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetPurchaseDemandDetail>>(pendingPurchaseDemands).ToList();
            return PurchaseDemandDetail;
        }
    }
}

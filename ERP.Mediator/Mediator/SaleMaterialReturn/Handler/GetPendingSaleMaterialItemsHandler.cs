using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class GetPendingSaleMaterialItemsHandler : IRequestHandler<GetPendingSaleMaterialItemsQuery, List<GetSaleMaterialDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingSaleMaterialItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetSaleMaterialDetail>> Handle(GetPendingSaleMaterialItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<SaleMaterialDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<SaleMaterialDetail, object>>[] includes = {
                x => x.Item
             };

            Expression<Func<SaleMaterialDetail, bool>> predicate = x => x.IsActive == true
                   && x.SaleMaterialId == request.SaleMaterialId;

            // Fetch all active SaleMaterial for the current store, including necessary relationships
            var entity = unitOfWork.Repository<SaleMaterialDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var SaleMaterial = entity.Item1.ToList();
            // Use a HashSet to optimize lookups for processed PurchaseDemandDetail IDs
            var processedDetailIds = unitOfWork.Repository<SaleMaterialReturnDetail>()
                .FindAll(x => x.IsActive == true && x.SaleMaterialDetail.SaleMaterialId == request.SaleMaterialId)
                .GroupBy(pd => pd.SaleMaterialDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<SaleMaterialDetail> pendingSaleMaterial = new();

            if (request.SaleMaterialReturnId == 0)
            {
                // Filter PurchaseDemandDetails that still have pending quantities
                pendingSaleMaterial = SaleMaterial
                   .Where(detail =>
                   {
                       // Check if there is a processed quantity for this detail, otherwise assume 0
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                       // Check if the required quantity is greater than the total Ordered quantity (pending)
                       return detail.Quantity > totalOrderedQty;
                   })
                   .ToList();  // Ensure you get the filtered list

                // Update the Required quantity by subtracting the OrderedQty
                foreach (var detail in pendingSaleMaterial)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Quantity -= (long)totalOrderedQty;  // Subtract the Ordered quantity from the required quantity
                }
            }
            else
            {
                pendingSaleMaterial = SaleMaterial;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetSaleMaterialDetail>>(pendingSaleMaterial).ToList();
            return PurchaseDemandDetail;
        }
    }
}

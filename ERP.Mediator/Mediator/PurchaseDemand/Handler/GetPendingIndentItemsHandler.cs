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
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetPendingIndentItemsHandler : IRequestHandler<GetPendingIndentItemsQuery, List<GetIndentRequestDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingIndentItemsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetIndentRequestDetail>> Handle(GetPendingIndentItemsQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression
            Expression<Func<IndentRequestDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<IndentRequestDetail, object>> includeProperties = x => x.Item.UOM;

            Expression<Func<IndentRequestDetail, object>>[] includes = {
                x => x.Item,
                x => x.Item.UOM
             };

            Expression<Func<IndentRequestDetail, bool>> predicate = x => x.IsActive == true
                   && x.IndentRequestId == request.IndentRequestId;

            //// Fetch all active IndentRequests for the current store, including necessary relationships
            //var entity = unitOfWork.Repository<IndentRequestDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            //var indentRequests = entity.Item1.ToList();
            //// Use a HashSet to optimize lookups for processed IndentRequestDetail IDs
            //var processedDetailIds =  unitOfWork.Repository<PurchaseDemandDetail>()
            //    .FindAll(x=> x.IsActive == true 
            //    && x.PurchaseDemand.IndentRequestId == request.IndentRequestId)
            //    .GroupBy(pd => pd.IndentRequestDetailId)
            //    .Select(g => new { DetailId = g.Key, TotalDemandedQty = g.Sum(pd => pd.DemandQty) })
            //    .ToDictionary(x => x.DetailId, x => x.TotalDemandedQty);

            List<IndentRequestDetail> pendingIndentRequests = new();

            //if(request.PurchaseDemandId == 0)
            //{
            //    // Filter IndentRequestDetails that still have pending quantities
            //     pendingIndentRequests = indentRequests
            //        .Where(detail =>
            //        {
            //            // Check if there is a processed quantity for this detail, otherwise assume 0
            //            var totalDemandedQty = processedDetailIds.TryGetValue(detail.Id, out var demandedQty) ? demandedQty : 0;

            //            // Check if the required quantity is greater than the total demanded quantity (pending)
            //            return detail.Required > totalDemandedQty;
            //        })
            //        .ToList();  // Ensure you get the filtered list
            //}
            //else
            //{
            //    pendingIndentRequests = indentRequests;
            //}

            //// Update the Required quantity by subtracting the DemandedQty
            //foreach (var detail in pendingIndentRequests)
            //{
            //    var totalDemandedQty = processedDetailIds.TryGetValue(detail.Id, out var demandedQty) ? demandedQty : 0;
            //    detail.Required -= totalDemandedQty;  // Subtract the demanded quantity from the required quantity
            //}

            var IndentRequestDetail = mapper.Map<IEnumerable<GetIndentRequestDetail>>(pendingIndentRequests).ToList();
            return IndentRequestDetail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetPendingIndentRequestHandler : IRequestHandler<GetPendingIndentRequestQuery, List<GetDropDown>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingIndentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetDropDown>> Handle(GetPendingIndentRequestQuery request, CancellationToken cancellationToken)
        {
            //// Define the ordering expression for the IndentRequests (sorted by ApprovedDate descending)
            //Expression<Func<Entities.Models.IndentRequest, object>> orderByExpression = x => x.ApprovedDate;

            //// Fetch all active IndentRequests for the current store, including necessary relationships (e.g., IndentRequestDetail, IndentType)
            //var indentRequests = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetAsync(
            //    x => x.IsActive && // Ensure IndentRequest is active
            //         x.StatusId == 3 && // Ensure StatusId is 3 (pending or processed, depending on business rules)
            //         x.StoreId == sessionProvider.Session.StoreId, // Ensure StoreId matches the current session's store
            //    includeProperties: "IndentRequestDetail,IndentType", // Include related entities (IndentRequestDetail and IndentType)
            //    orderByDec: query => query.OrderByDescending(orderByExpression) // Order by ApprovedDate descending
            //);

            //// Fetch all processed PurchaseDemandDetail records and calculate the total demanded quantities for each IndentRequestDetailId
            //var processedDetailIds = unitOfWork.Repository<PurchaseDemandDetail>()
            //    .GetAll() // Get all PurchaseDemandDetail records
            //    .GroupBy(pd => pd.IndentRequestDetailId) // Group by IndentRequestDetailId to sum the quantities
            //    .Select(g => new
            //    {
            //        DetailId = g.Key,
            //        TotalDemandedQty = g.Sum(pd => pd.DemandQty) // Calculate the total demanded quantity for each IndentRequestDetailId
            //    })
            //    .ToDictionary(x => x.DetailId, x => x.TotalDemandedQty); // Map to dictionary for fast lookup

            // List to store pending IndentRequests
            List<Entities.Models.IndentRequest> pendingIndentRequests = new();

            //// Iterate through each IndentRequest
            //foreach (var item in indentRequests)
            //{
            //    if(item.Id != request.IndentRequestId)
            //    {
            //        bool hasPendingDetail = false; // Flag to track if any detail has a pending quantity

            //        // Iterate through each IndentRequestDetail for the current IndentRequest
            //        foreach (var detail in item.IndentRequestDetail.Where(y => y.IsActive))
            //        {
            //            // Fetch the total demanded quantity for this detail (if exists, otherwise default to 0)
            //            var totalDemandedQty = processedDetailIds.TryGetValue(detail.Id, out var demandedQty) ? demandedQty : 0;

            //            // Check if the required quantity is greater than the demanded quantity
            //            if (detail.Required > totalDemandedQty)
            //            {
            //                hasPendingDetail = true; // Mark this IndentRequest as having a pending detail
            //                break; // Exit the inner loop as we already know this IndentRequest has pending quantity
            //            }
            //        }

            //        // If there is any detail with a pending quantity, add the IndentRequest to the result list
            //        if (hasPendingDetail)
            //        {
            //            pendingIndentRequests.Add(item);
            //        }
            //    }
            //    else
            //        pendingIndentRequests.Add(item);
            //}

            // Map the filtered IndentRequests to GetDropDown DTOs and return the result
            return mapper.Map<List<GetDropDown>>(pendingIndentRequests);
        }
    }
}

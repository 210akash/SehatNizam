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
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class GetPendingDemandHandler : IRequestHandler<GetPendingDemandQuery, List<GetDropDown>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingDemandHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetDropDown>> Handle(GetPendingDemandQuery request, CancellationToken cancellationToken)
        {
            // Define the ordering expression for the PurchaseDemands (sorted by ApprovedDate descending)
            Expression<Func<Entities.Models.PurchaseDemand, object>> orderByExpression = x => x.ApprovedDate;

            // Fetch all active PurchaseDemands for the current store, including necessary relationships (e.g., PurchaseDemandDetail, IndentType)
            var PurchaseDemands = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetAsync(
                x => x.IsActive && // Ensure PurchaseDemand is active
                     x.StatusId == 3, //&& // Ensure StatusId is 3 (pending or processed, depending on business rules)
                     //x. == sessionProvider.Session.StoreId, // Ensure StoreId matches the current session's store
                includeProperties: "PurchaseDemandDetail", // Include related entities (PurchaseDemandDetail and IndentType)
                orderByDec: query => query.OrderByDescending(orderByExpression) // Order by ApprovedDate descending
            );

            // Fetch all processed PurchaseOrderDetail records and calculate the total Ordered quantities for each PurchaseDemandDetailId
            var processedDetailIds = unitOfWork.Repository<PurchaseOrderDetail>()
                .GetAll() // Get all PurchaseOrderDetail records
                .GroupBy(pd => pd.PurchaseDemandDetailId) // Group by PurchaseDemandDetailId to sum the quantities
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalOrderedQty = g.Sum(pd => pd.Quantity) // Calculate the total Ordered quantity for each PurchaseDemandDetailId
                })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty); // Map to dictionary for fast lookup

            // List to store pending PurchaseDemands
            List<Entities.Models.PurchaseDemand> pendingPurchaseDemands = new();

            // Iterate through each PurchaseDemand
            foreach (var item in PurchaseDemands)
            {
                if(item.Id != request.PurchaseDemandId)
                {
                    bool hasPendingDetail = false; // Flag to track if any detail has a pending quantity

                    // Iterate through each PurchaseDemandDetail for the current PurchaseDemand
                    foreach (var detail in item.PurchaseDemandDetail.Where(y => y.IsActive))
                    {
                        // Fetch the total Ordered quantity for this detail (if exists, otherwise default to 0)
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        // Check if the required quantity is greater than the Ordered quantity
                        if (detail.DemandQty > totalOrderedQty)
                        {
                            hasPendingDetail = true; // Mark this PurchaseDemand as having a pending detail
                            break; // Exit the inner loop as we already know this PurchaseDemand has pending quantity
                        }
                    }

                    // If there is any detail with a pending quantity, add the PurchaseDemand to the result list
                    if (hasPendingDetail)
                    {
                        pendingPurchaseDemands.Add(item);
                    }
                }
                else
                    pendingPurchaseDemands.Add(item);
            }

            // Map the filtered PurchaseDemands to GetDropDown DTOs and return the result
            return mapper.Map<List<GetDropDown>>(pendingPurchaseDemands);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetPendingCostSheetByItemHandler : IRequestHandler<GetPendingCostSheetByItemQuery, List<GetCostSheet>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingCostSheetByItemHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;

        }

        public async Task<List<GetCostSheet>> Handle(GetPendingCostSheetByItemQuery request, CancellationToken cancellationToken)
        {
            request.ProjectId = sessionProvider.Session.SelectedWarehouseId != 0
                ? sessionProvider.Session.SelectedWarehouseId
                : request.ProjectId;

            // Fetch all CostSheets for the item with related details
            var costSheets = await unitOfWork.Repository<Entities.Models.CostSheet>()
                .GetAsync(
                    cs => cs.ItemId == request.ItemId && cs.IsActive,
                    null,
                    includeProperties: "GRNDetails,GRNDetails.GRN," +
                    "GRNDetails.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail," +
                    "DispatchDetail,DispatchDetail.DispatchOrder.Dispatch," +
                    "WarehouseTransferDetail," +
                    "WarehouseTransferDetail.WarehouseTransfer"
                );

            List<Entities.Models.CostSheet> pendingCostSheets = new();

            foreach (var cs in costSheets)
            {
                // Received Qty (GRNs approved for this project)
                var receivedQty = cs.GRNDetails
                    .Where(grn => grn.IsActive &&
                                  grn.GRN.IsActive &&
                                  grn.GRN.StatusId == (long)OrderStatusEnum.Approved &&
                                  grn.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.ProjectId == request.ProjectId)
                    .Sum(grn => grn.Received);

                // Transfer In Qty
                var transferInQty = cs.WarehouseTransferDetail
                .Where(tr => tr.IsActive &&
                                 tr.WarehouseTransfer.IsActive &&
                                 tr.WarehouseTransfer.TransferToId == request.ProjectId &&
                                 tr.WarehouseTransfer.StatusId == (long)OrderStatusEnum.Approved)
                    .Sum(tr => tr.Quantity);

                // Transfer Out Qty
                var transferOutQty = cs.WarehouseTransferDetail
                    .Where(tr => tr.IsActive &&
                                 tr.WarehouseTransfer.IsActive &&
                                 tr.WarehouseTransfer.TransferFromId == request.ProjectId &&
                                 tr.WarehouseTransfer.StatusId == (long)OrderStatusEnum.Approved)
                    .Sum(tr => tr.Quantity);

                // Dispatched Qty
                var dispatchedQty = cs.DispatchDetail
                    .Where(d => d.IsActive &&
                                d.DispatchOrder.Dispatch.IsActive &&
                                d.DispatchOrder.Dispatch.ProjectId == request.ProjectId &&
                                d.DispatchOrder.Dispatch.StatusId == (long)OrderStatusEnum.Approved)
                    .Sum(d => d.Quantity);

                // Calculate pending
                var pendingQty = (receivedQty + transferInQty - transferOutQty) - dispatchedQty;

                if (pendingQty > 0)
                {
                    cs.Quantity = pendingQty; // only keep pending qty
                    pendingCostSheets.Add(cs);
                }
            }

            return mapper.Map<List<GetCostSheet>>(pendingCostSheets);
        }

        public async Task<List<GetCostSheet>> Handle1(GetPendingCostSheetByItemQuery request, CancellationToken cancellationToken)
        {
            //// 1. Get approved cost sheets for the requested item
            //var costSheets = await unitOfWork
            //    .Repository<Entities.Models.CostSheet>()
            //    .GetAsync(
            //        cs => cs.IsActive &&
            //              cs.ItemId == request.ItemId &&
            //              cs.StatusId == (long)OrderStatusEnum.Approved,
            //        orderByDec: q => q.OrderByDescending(cs => cs.CreatedDate)
            //    );

            //// 2. Get dispatched quantities per CostSheetId
            //var dispatchedQuery = unitOfWork.Repository<DispatchDetail>()
            //    .GetAll(null, null, "DispatchOrder,DispatchOrder.Dispatch,OrderItem")
            //    .Where(d =>
            //        d.IsActive &&
            //        d.DispatchOrder.Dispatch.IsActive &&
            //        d.DispatchOrder.Dispatch.StatusId == 3 && // Approved dispatch
            //        d.OrderItem.ItemId == request.ItemId
            //    );

            //// Exclude current dispatch if provided
            //if (request.DispatchId != null && request.DispatchId != 0)
            //{
            //    dispatchedQuery = dispatchedQuery.Where(d => d.DispatchOrder.DispatchId != request.DispatchId);
            //}

            //var dispatchedLookup =  dispatchedQuery
            //    .GroupBy(d => d.CostSheetId)
            //    .Select(g => new
            //    {
            //        CostSheetId = g.Key,
            //        Qty = g.Sum(d => d.Quantity)
            //    })
            //    .ToDictionary(x => x.CostSheetId, x => x.Qty);

            //// 3. Filter cost sheets with pending quantity and calculate remaining
            //var pendingCostSheets = costSheets
            //    .Where(cs =>
            //    {
            //        dispatchedLookup.TryGetValue(cs.Id, out var dispatchedQty);
            //        return dispatchedQty < cs.Quantity;
            //    })
            //    .ToList();

            //// 4. Map to DTOs
            //var result = mapper.Map<List<GetCostSheet>>(pendingCostSheets);

            //// 5. Set RemainingQty for each result
            //foreach (var dto in result)
            //{
            //    dispatchedLookup.TryGetValue(dto.Id, out var dispatchedQty);
            //    dto.Quantity = dto.Quantity - dispatchedQty;
            //}

            //return result;

            // Define the ordering expression for the PurchaseDemands (sorted by ApprovedDate descending)
            Expression<Func<Entities.Models.CostSheet, object>> orderByExpression = x => x.ApprovedDate;

            // Fetch all active CostSheets for the Item, including necessary relationships (e.g., PurchaseDemandDetail, IndentType)
            //var costSheets = await unitOfWork
            //    .Repository<Entities.Models.CostSheet>()
            //    .GetAsync(
            //        cs => cs.IsActive &&
            //              cs.ItemId == request.ItemId &&
            //              cs.StatusId == (long)OrderStatusEnum.Approved,
            //        orderByDec: q => q.OrderByDescending(cs => cs.CreatedDate)
            //    );

            request.ProjectId = sessionProvider.Session.SelectedWarehouseId != 0 ? sessionProvider.Session.SelectedWarehouseId :
            request.ProjectId;
            var costSheets = await unitOfWork
           .Repository<GRNDetail>()
           .GetAsync(
               cs => cs.IsActive &&
                     cs.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.ItemId == request.ItemId &&
                     cs.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.ProjectId == request.ProjectId &&
                     cs.GRN.StatusId == (long)OrderStatusEnum.Approved,
                     null,
                     orderByDec: q => q.OrderByDescending(cs => cs.CreatedDate),
                     "CostSheet,GRN,InspectionDetail,InspectionDetail.IGPDetail,InspectionDetail.IGPDetail,InspectionDetail.IGPDetail.PurchaseOrderDetail,InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail"
           );

           // var costSheets = GRNDetails.Select(y=>y.CostSheet).ToList();

            // Fetch all processed PurchaseOrderDetail records and calculate the total Ordered quantities for each PurchaseDemandDetailId
            var processedDetailIds = unitOfWork.Repository<DispatchDetail>()
              // Get all PurchaseOrderDetail records
              .GetAll(null, null, "DispatchOrder,DispatchOrder.Dispatch,OrderItem")
                    .Where(d =>
                        d.IsActive &&
                        d.DispatchOrder.Dispatch.IsActive &&
                        d.DispatchOrder.Dispatch.ProjectId == request.ProjectId &&
                        //d.DispatchOrder.Dispatch.StatusId == 3 && // Approved dispatch
                        d.OrderItem.ItemId == request.ItemId
                    )
                .GroupBy(pd => pd.CostSheetId) // Group by PurchaseDemandDetailId to sum the quantities
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalDispatchQty = g.Sum(pd => pd.Quantity) // Calculate the total Ordered quantity for each PurchaseDemandDetailId
                })
                .ToDictionary(x => x.DetailId, x => x.TotalDispatchQty); // Map to dictionary for fast lookup

            // List to store pending PurchaseDemands
            List<Entities.Models.CostSheet> pendingCostSheets = new();

            // Iterate through each PurchaseDemand
            foreach (var item in costSheets)
            {
                if (item.CostSheetId != request.CostSheetId)
                {
                    bool hasPendingDetail = false; // Flag to track if any detail has a pending quantity

                    var totalOrderedQty = processedDetailIds.TryGetValue(item.CostSheetId, out var OrderedQty) ? OrderedQty : 0;
                    // Check if the required quantity is greater than the Ordered quantity
                    if (item.Received > totalOrderedQty)
                    {
                        item.CostSheet.Quantity = item.Received - totalOrderedQty;
                        hasPendingDetail = true; // Mark this PurchaseDemand as having a pending detail
                    }

                    // If there is any detail with a pending quantity, add the PurchaseDemand to the result list
                    if (hasPendingDetail)
                    {
                        pendingCostSheets.Add(item.CostSheet);
                    }
                }
                else
                {
                     var totalOrderedQty = processedDetailIds.TryGetValue(item.CostSheetId, out var OrderedQty) ? OrderedQty : 0;
                     item.CostSheet.Quantity = item.Received - totalOrderedQty;
                     pendingCostSheets.Add(item.CostSheet);
                }
            }
            // Map the filtered PurchaseDemands to GetDropDown DTOs and return the result
            return mapper.Map<List<GetCostSheet>>(pendingCostSheets);
        }
    }
}

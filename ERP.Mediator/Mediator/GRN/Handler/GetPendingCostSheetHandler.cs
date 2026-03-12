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
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetPendingCostSheetHandler : IRequestHandler<GetPendingCostSheetQuery, List<GetDropDown>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingCostSheetHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDropDown>> Handle(GetPendingCostSheetQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.CostSheet, object>> orderByExpression = x => x.CreatedDate;

            // Fetch matching cost sheets
            var costSheets = await unitOfWork.Repository<Entities.Models.CostSheet>().GetAsync(
                x => x.IsActive &&
                     x.ItemId == request.ItemId &&
                     x.StatusId == 3,
                includeProperties: "GRNDetails", // Include GRNDetails if used below
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            // Filter out GRNDetails with null CostSheetId before grouping
            var usedInspectionDetailIds = unitOfWork.Repository<GRNDetail>()
                .GetAll(null, null, "InspectionDetail,InspectionDetail.IGPDetail,InspectionDetail.IGPDetail.PurchaseOrderDetail,InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail")
                .Where(x =>
                    x.IsActive &&
                    x.CostSheetId.HasValue &&
                    x.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.ItemId == request.ItemId)
                .GroupBy(pd => pd.CostSheetId.Value)
                .Select(g => new
                {
                    CostSheetId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Received)
                })
                .ToDictionary(x => x.CostSheetId, x => x.TotalReceivedQty);

            List<Entities.Models.CostSheet> pendingInspection = new();

            foreach (var sheet in costSheets)
            {
                // Always include the current CostSheet being edited
                if (sheet.Id == request.CostSheetId)
                {
                    pendingInspection.Add(sheet);
                    continue;
                }

                var totalReceivedQty = usedInspectionDetailIds.TryGetValue(sheet.Id, out var qty) ? qty : 0;

                if (sheet.Quantity > totalReceivedQty)
                {
                    pendingInspection.Add(sheet);
                }
            }

            return mapper.Map<List<GetDropDown>>(pendingInspection);
        }
    }
}

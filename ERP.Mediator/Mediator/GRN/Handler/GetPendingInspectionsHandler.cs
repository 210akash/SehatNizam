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
    public class GetPendingInspectionsHandler : IRequestHandler<GetPendingInspectionsQuery, List<GetInspection>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingInspectionsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetInspection>> Handle(GetPendingInspectionsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Inspection, object>> orderByExpression = x => x.CreatedDate;

            var Inspections = await unitOfWork.Repository<Entities.Models.Inspection>().GetAsync(
                x => x.IsActive &&
                      x.IGP.ProjectId == sessionProvider.Session.SelectedWarehouseId &&
                     x.StatusId == 3,

                includeProperties: "InspectionDetail,InspectionDetail.IGPDetail,IGP,IGP.PurchaseOrder,IGP.PurchaseOrder.Vendor,IGP.PurchaseOrder.PurchaseOrderDetail,IGP.PurchaseOrder.PurchaseOrderDetail.PurchaseDemandDetail.PurchaseDemand,IGP.PurchaseOrder.PurchaseOrderDetail.PurchaseDemandDetail.PurchaseDemand.IndentRequest",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var processedDetailIds = unitOfWork.Repository<GRNDetail>()
                .GetAll().Where(x => x.IsActive)
                .GroupBy(pd => pd.InspectionDetailId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Received)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<Entities.Models.Inspection> pendingInspection = new();

            foreach (var item in Inspections)
            {
                if (item.Id != request.InspectionId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.InspectionDetail.Where(y => y.IsActive))
                    {
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        if (detail.IGPDetail.Received - detail.Rejected > totalOrderedQty)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingInspection.Add(item);
                    }
                }
                else
                    pendingInspection.Add(item);
            }

            return mapper.Map<List<GetInspection>>(pendingInspection);
        }
    }
}

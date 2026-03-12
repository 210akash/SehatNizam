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
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class GetPendingPurchaseOrdersHandler : IRequestHandler<GetPendingPurchaseOrdersQuery, List<GetPurchaseOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingPurchaseOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetPurchaseOrder>> Handle(GetPendingPurchaseOrdersQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.PurchaseOrder, object>> orderByExpression = x => x.ApprovedDate;

            var PurchaseOrders = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetAsync(
                x => x.IsActive &&
                     x.PurchaseOrderDetail.Any(y=> y.PurchaseDemandDetail.ProjectId == sessionProvider.Session.SelectedWarehouseId) &&
                     x.StatusId == 3,
                includeProperties: "Vendor,PurchaseOrderDetail,PurchaseOrderDetail.PurchaseDemandDetail",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var processedDetailIds = unitOfWork.Repository<IGPDetails>()
                .GetAll().Where(x => x.IsActive)
                .GroupBy(pd => pd.PurchaseOrderDetailId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Received)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<Entities.Models.PurchaseOrder> pendingPurchaseOrder = new();

            foreach (var item in PurchaseOrders)
            {
                if (item.Id != request.PurchaseOrderId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.PurchaseOrderDetail.Where(y => y.IsActive))
                    {
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        if (detail.Quantity > totalOrderedQty)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingPurchaseOrder.Add(item);
                    }
                }
                else
                    pendingPurchaseOrder.Add(item);
            }

            return mapper.Map<List<GetPurchaseOrder>>(pendingPurchaseOrder);
        }
    }
}

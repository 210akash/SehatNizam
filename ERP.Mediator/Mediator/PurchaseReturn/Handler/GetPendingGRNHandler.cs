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
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class GetPendingGRNHandler : IRequestHandler<GetPendingGRNQuery, List<GetGRN>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingGRNHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetGRN>> Handle(GetPendingGRNQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ERP.Entities.Models.GRN, object>> orderByExpression = x => x.CreatedDate;
            var rawOrders = await unitOfWork.Repository<ERP.Entities.Models.GRN>().GetAsync(
                x => x.IsActive 
                && x.Inspection.IGP.PurchaseOrder.PurchaseOrderDetail.Any(y=>y.PurchaseDemandDetail.ProjectId == sessionProvider.Session.SelectedWarehouseId)
                && x.StatusId == 3,
                includeProperties: "GRNDetail,Inspection,Inspection.IGP,Inspection.IGP.PurchaseOrder,Inspection.IGP.PurchaseOrder.Vendor,Inspection.IGP.PurchaseOrder.PurchaseOrderDetail,Inspection.IGP.PurchaseOrder.PurchaseOrderDetail.PurchaseDemandDetail",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var searchParam = request.searchParam?.Trim();
            var PurchaseOrders = string.IsNullOrWhiteSpace(searchParam)
         ? rawOrders.Take(10).ToList()
         : rawOrders
             .Where(x =>
             {
                 var digitsOnly = new string(x.Code.Where(char.IsDigit).ToArray());
                 return digitsOnly.Contains(searchParam);
             })
             .Take(10)
             .ToList();

            var processedDetailIds = unitOfWork.Repository<PurchaseReturnDetail>()
                .GetAll().Where(x => x.IsActive)
                .GroupBy(pd => pd.GRNDetailId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<ERP.Entities.Models.GRN> pendingPurchaseOrder = new();

            foreach (var item in PurchaseOrders)
            {
                if (item.Id != request.GRNId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.GRNDetail.Where(y => y.IsActive))
                    {
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        if (detail.Received > totalOrderedQty)
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

            return mapper.Map<List<GetGRN>>(pendingPurchaseOrder);
        }
    }
}

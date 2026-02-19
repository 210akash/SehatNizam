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

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class GetPendingPOItemsHandler : IRequestHandler<GetPendingPOItemsQuery, List<GetPurchaseOrderDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingPOItemsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPurchaseOrderDetail>> Handle(GetPendingPOItemsQuery request, CancellationToken cancellationToken)
        {
            var purchaseDemandIds = unitOfWork.Repository<PurchaseOrderDetail>().GetAsync(x => x.PurchaseOrderId == request.PurchaseOrderId).Result.Select(x => x.PurchaseDemandDetailId);

            Expression<Func<PurchaseOrderDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<PurchaseOrderDetail, object>>[] includes = {
                x => x.PurchaseOrder,
                x => x.PurchaseDemandDetail,
                x => x.PurchaseDemandDetail.Item
             };

            Expression<Func<PurchaseOrderDetail, bool>> predicate = x => x.IsActive == true
            && x.PurchaseOrderId == request.PurchaseOrderId
            && purchaseDemandIds.Contains(x.PurchaseDemandDetailId);

            var entity = unitOfWork.Repository<PurchaseOrderDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var purchaseOrderDetails = entity.Item1.ToList();

            var processedDetailIds = unitOfWork.Repository<PurchaseOrderDetail>()
                .FindAll(x => x.IsActive == true && purchaseDemandIds.Contains(x.PurchaseDemandDetail.Id))
                .GroupBy(pd => pd.PurchaseDemandDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<PurchaseOrderDetail> pendingPurchaseOrderDetail = new();


            if (request.PurchaseOrderId == 0)
            {
                pendingPurchaseOrderDetail = purchaseOrderDetails
                   .Where(detail =>
                   {
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                       return detail.PurchaseDemandDetail.DemandQty > totalOrderedQty;
                   })
                   .ToList();
            }
            else
            {
                pendingPurchaseOrderDetail = purchaseOrderDetails;
            }

            foreach (var detail in pendingPurchaseOrderDetail)
            {
                var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                detail.PurchaseDemandDetail.DemandQty -= totalOrderedQty;
            }

            var PurchaseOrderDetail = mapper.Map<IEnumerable<GetPurchaseOrderDetail>>(pendingPurchaseOrderDetail).ToList();
            return PurchaseOrderDetail;
        }
    }
}

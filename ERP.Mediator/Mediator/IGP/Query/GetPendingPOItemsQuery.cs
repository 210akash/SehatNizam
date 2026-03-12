using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class GetPendingPOItemsQuery : IRequest<List<GetPurchaseOrderDetail>>
    {
        public GetPendingPOItemsQuery(long PurchaseOrderId,long IGPId)
        {
            this.PurchaseOrderId = PurchaseOrderId;
            this.IGPId = IGPId;
        }

        public long PurchaseOrderId { get; set; }
        public long IGPId { get; set; }
    }
}
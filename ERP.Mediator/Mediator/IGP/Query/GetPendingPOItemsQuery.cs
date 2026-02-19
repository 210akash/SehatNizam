using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class GetPendingPOItemsQuery : IRequest<List<GetPurchaseOrderDetail>>
    {
        public GetPendingPOItemsQuery(long PurchaseOrderId)
        {
            this.PurchaseOrderId = PurchaseOrderId;
        }

        public long PurchaseOrderId { get; set; }
    }
}
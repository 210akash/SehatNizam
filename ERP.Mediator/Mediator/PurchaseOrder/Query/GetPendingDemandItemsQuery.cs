using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class GetPendingDemandItemsQuery : IRequest<List<GetPurchaseDemandDetail>>
    {
        public GetPendingDemandItemsQuery(long PurchaseDemandId, long PurchaseOrderId,long VendorId)
        {
            this.PurchaseDemandId = PurchaseDemandId;
            this.PurchaseOrderId = PurchaseOrderId;
            this.VendorId = VendorId;
        }

        public long PurchaseDemandId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long VendorId { get; set; }
    }
}
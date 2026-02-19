using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetIGP
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long PurchaseOrderId { get; set; }
        public virtual GetPurchaseOrder PurchaseOrder { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetIGPDetails> IGPDetails { get; set; }
    }

    public class GetIGPDetails
    {
        public long Id { get; set; }

        public long IGPId { get; set; }
        public virtual GetIGP IGP { get; set; }

        public decimal Received { get; set; }

        public long PurchaseOrderDetailId { get; set; }
        public virtual GetPurchaseOrderDetail PurchaseOrderDetail { get; set; }
    }
}

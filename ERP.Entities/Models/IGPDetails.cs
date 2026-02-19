using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class IGPDetails : BaseEntity
    {
        public long IGPId { get; set; }
        public virtual IGP IGP { get; set; }

        public decimal Received { get; set; }

        public long PurchaseOrderDetailId { get; set; }
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }
    }
}

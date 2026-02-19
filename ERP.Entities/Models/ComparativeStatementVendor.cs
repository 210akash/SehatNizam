using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ComparativeStatementVendor : BaseEntity
    {
        public long ComparativeStatementDetailId { get; set; }
        public virtual ComparativeStatementDetail ComparativeStatementDetail { get; set; }

        public long VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public decimal Price { get; set; }

        public virtual List<PurchaseOrderDetail> PurchaseOrderDetail { get; set; }
    }
}

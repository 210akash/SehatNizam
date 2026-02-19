using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetComparativeStatement
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public string Remarks { get; set; }
        public long PurchaseDemandId { get; set; }
        public GetPurchaseDemand PurchaseDemand { get; set; }

        public virtual List<GetComparativeStatementDetail> ComparativeStatementDetail { get; set; }
    }

    public class GetComparativeStatementDetail
    {
        public long Id { get; set; }
        public long ComparativeStatementId { get; set; }

        public long PurchaseDemandDetailId { get; set; }
        public GetPurchaseDemandDetail PurchaseDemandDetail { get; set; }

        public decimal Quantity { get; set; }

        public virtual List<GetComparativeStatementVendor> ComparativeStatementVendor { get; set; }
    }

    public class GetComparativeStatementVendor
    {
        public long Id { get; set; }
        public long ComparativeStatementDetailId { get; set; }

        public long VendorId { get; set; }
        public virtual GetVendor Vendor { get; set; }

        public decimal Price { get; set; }
    }
}

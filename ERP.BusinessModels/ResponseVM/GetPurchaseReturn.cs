using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPurchaseReturn
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long GRNId { get; set; }
        public virtual GetGRN GRN { get; set; }

        public long? ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetPurchaseReturnDetail> PurchaseReturnDetail { get; set; }
    }

    public class GetPurchaseReturnDetail
    {
        public long Id { get; set; }
        public long PurchaseReturnId { get; set; }
        public virtual GetPurchaseReturn PurchaseReturn { get; set; }
        public decimal Quantity { get; set; }
        public long GRNDetailId { get; set; }
        public virtual GetGRNDetail GRNDetail { get; set; }
    }
}

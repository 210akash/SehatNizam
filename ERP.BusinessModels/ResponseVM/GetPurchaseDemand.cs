using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPurchaseDemand
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public string RequestNo { get; set; }
        public DateTime RequestDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long IndentTypeId { get; set; }
        public GetIndentType IndentType { get; set; }

        public long PriorityId { get; set; }
        public virtual GetPriority Priority { get; set; }

        public long LocationId { get; set; }
        public virtual GetLocation Location { get; set; }

        public long? StoreId { get; set; }
        public virtual GetStore Store { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetPurchaseDemandDetail> PurchaseDemandDetail { get; set; }
    }

    public class GetPurchaseDemandDetail
    {
        public long Id { get; set; }
        public long PurchaseDemandId { get; set; }
        public GetPurchaseDemand PurchaseDemand { get; set; }

        public long ItemId { get; set; }
        public virtual GetItem Item { get; set; }

        public long? ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public long? DepartmentId { get; set; }
        public virtual GetDepartment Department { get; set; }

        public virtual GetComparativeStatementDetail ComparativeStatementDetail { get; set; }

        public DateTime RequiredDate { get; set; }
        public decimal DemandQty { get; set; }
        public string Description { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetIssuance
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ProcessedDate { get; set; }
        public GetUser ProcessedBy { get; set; }

        public DateTime ApprovedDate { get; set; }
        public GetUser ApprovedBy { get; set; }

        public DateTime Date { get; set; }

        public long IndentRequestId { get; set; }
        public virtual GetIndentRequest IndentRequest { get; set; }

        public long ProjectId { get; set; }
        public virtual GetProject Project { get; set; }
        
        public virtual GetAccount Account { get; set; }
        public long? AccountId { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetIssuanceDetail> IssuanceDetail { get; set; }
    }

    public class GetIssuanceDetail
    {
        public long Id { get; set; }

        public long IssuanceId { get; set; }
        public virtual GetIssuance Issuance { get; set; }

        public long ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public long IndentRequestDetailId { get; set; }
        public virtual GetIndentRequestDetail IndentRequestDetail { get; set; }

        public long? CostSheetId { get; set; }
        public virtual GetCostSheet CostSheet { get; set; }

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}

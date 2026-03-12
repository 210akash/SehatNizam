using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetInspection
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long IGPId { get; set; }
        public virtual GetIGP IGP { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetInspectionDetail> InspectionDetail { get; set; }
    }

    public class GetInspectionDetail
    {
        public long Id { get; set; }

        public long InspectionId { get; set; }
        public virtual GetInspection Inspection { get; set; }

        public long RejectReasonId { get; set; }
        public GetRejectReason RejectReason { get; set; }
        public decimal Rejected { get; set; }
        public decimal Approved { get; set; }
        public string Remarks { get; set; }

        public long IGPDetailId { get; set; }
        public virtual GetIGPDetails IGPDetail { get; set; }
    }
}

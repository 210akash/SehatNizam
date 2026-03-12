using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public class InspectionDetail : BaseEntity
    {
        public long InspectionId { get; set; }
        public virtual Inspection Inspection { get; set; }

        public long IGPDetailId { get; set; }
        public virtual IGPDetails IGPDetail { get; set; }

        public long? RejectReasonId { get; set; }
        public virtual RejectReason RejectReason { get; set; }

        public decimal Rejected { get; set; }

        [NotMapped]
        public decimal Approved { get; set; }
        public string Remarks { get; set; }
    }
}

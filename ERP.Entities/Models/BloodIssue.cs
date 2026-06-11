using System;

namespace ERP.Entities.Models
{
    public class BloodIssue : BaseEntity
    {
        public long BloodRequestId { get; set; }
        public virtual BloodRequest BloodRequest { get; set; }
        public long BloodUnitId { get; set; }
        public virtual BloodUnit BloodUnit { get; set; }
        public long? BloodCrossMatchId { get; set; }
        public virtual BloodCrossMatch BloodCrossMatch { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedTo { get; set; }
        public string Remarks { get; set; }
    }
}

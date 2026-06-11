using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodIssue
    {
        public long Id { get; set; }
        public long BloodRequestId { get; set; }
        public GetBloodRequest BloodRequest { get; set; }
        public long BloodUnitId { get; set; }
        public GetBloodUnit BloodUnit { get; set; }
        public long? BloodCrossMatchId { get; set; }
        public GetBloodCrossMatch BloodCrossMatch { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedTo { get; set; }
        public string Remarks { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}

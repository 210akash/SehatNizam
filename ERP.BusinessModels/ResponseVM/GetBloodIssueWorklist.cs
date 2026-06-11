using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodIssueWorklist
    {
        public long BloodRequestId { get; set; }
        public GetBloodRequest BloodRequest { get; set; }
        public long BloodCrossMatchId { get; set; }
        public DateTime? CrossMatchDate { get; set; }
        public long BloodUnitId { get; set; }
        public GetBloodUnit BloodUnit { get; set; }
    }
}

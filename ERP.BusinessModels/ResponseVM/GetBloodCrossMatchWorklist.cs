using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodCrossMatchWorklist
    {
        public long CrossMatchId { get; set; }
        public long BloodRequestId { get; set; }
        public GetBloodRequest BloodRequest { get; set; }
        public long BloodUnitId { get; set; }
        public GetBloodUnit BloodUnit { get; set; }
        public DateTime? CrossMatchDate { get; set; }
        public int Result { get; set; }
        public string Remarks { get; set; }
    }
}

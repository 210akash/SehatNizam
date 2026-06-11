using System;

namespace ERP.Entities.Models
{
    public class BloodCrossMatch : BaseEntity
    {
        public long BloodRequestId { get; set; }
        public virtual BloodRequest BloodRequest { get; set; }
        public long BloodUnitId { get; set; }
        public virtual BloodUnit BloodUnit { get; set; }
        public DateTime CrossMatchDate { get; set; }
        public int Result { get; set; }
        public string Remarks { get; set; }
    }
}

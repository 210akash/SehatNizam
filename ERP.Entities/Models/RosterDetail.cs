using System;

namespace ERP.Entities.Models
{
    public class RosterDetail : BaseEntity
    {
        public long RosterId { get; set; }
        public virtual Roster Roster { get; set; }

        public Guid EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }

        public long EmployeeShiftId { get; set; }
        public EmployeeShift EmployeeShift { get; set; }

        // Date for this roster entry
        public DateTime RosterDate { get; set; }

        // Day status
        public bool IsOffDay { get; set; } = false;
    }
}

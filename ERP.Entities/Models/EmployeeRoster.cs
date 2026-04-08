using System;

namespace ERP.Entities.Models
{
    public class EmployeeRoster : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }

        public long EmployeeShiftId { get; set; }
        public EmployeeShift EmployeeShift { get; set; }

        // Date for this roster entry
        public DateTime RosterDate { get; set; }
    }
}

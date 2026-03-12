using System;

namespace ERP.Entities.Models
{
    public class EmployeeLeave : BaseEntityHistory
    {
        public Guid EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }
        public long EmployeeGroupLeaveTypeDetailId { get; set; }
        public virtual EmployeeGroupLeaveTypeDetail EmployeeGroupLeaveTypeDetail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsFirstHalfDay { get; set; } = false;
        public bool IsLastHalfDay { get; set; } = false;
        public string Reason { get; set; }
        public string Comments { get; set; }
        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

    }
}

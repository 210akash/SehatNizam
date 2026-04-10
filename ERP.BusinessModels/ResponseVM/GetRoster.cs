using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRoster
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long DepartmentId { get; set; }
        public virtual GetDepartment Department { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public GetCreatedBy CreatedBy { get; set; }

        public GetCreatedBy ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetCreatedBy ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public virtual List<GetRosterDetail> RosterDetail { get; set; }
    }

    public class GetRosterDetail
    {
        public long Id { get; set; }
        public long RosterId { get; set; }

        public Guid EmployeeId { get; set; }
        public virtual GetUser Employee { get; set; }

        public long EmployeeShiftId { get; set; }
        public GetEmployeeShift EmployeeShift { get; set; }

        // Date for this roster entry
        public DateTime RosterDate { get; set; }

        // Day status
        public bool IsOffDay { get; set; } = false;
    }
}

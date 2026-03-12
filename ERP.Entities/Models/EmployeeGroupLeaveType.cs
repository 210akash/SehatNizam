using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class EmployeeGroupLeaveType : BaseEntity
    {
        public long EmployeeLeaveGroupId { get; set; }
        public virtual EmployeeLeaveGroup EmployeeLeaveGroup { get; set; }
        public long HRYearId { get; set; }
        public virtual HRYear HRYear { get; set; }
        public List<EmployeeGroupLeaveTypeDetail> EmployeeGroupLeaveTypeDetail { get; set; }
    }
}

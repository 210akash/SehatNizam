namespace ERP.Entities.Models
{
    public class EmployeeGroupLeaveTypeDetail : BaseEntity
    {
        public long EmployeeGroupLeaveTypeId { get; set; }
        public virtual EmployeeGroupLeaveType EmployeeGroupLeaveType { get; set; }
        public long NoOfLeaves { get; set; }
        public long EmployeeLeaveTypeId { get; set; }
        public virtual EmployeeLeaveType EmployeeLeaveType { get; set; }
    }
}

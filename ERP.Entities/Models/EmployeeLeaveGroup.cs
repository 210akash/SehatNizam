using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class EmployeeLeaveGroup : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<EmployeeGroupLeaveType> EmployeeGroupLeaveType { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

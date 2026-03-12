using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class EmployeeOvertimeRate : BaseEntity
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

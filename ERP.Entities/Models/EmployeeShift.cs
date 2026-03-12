using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class EmployeeShift : BaseEntity
    {
        public string Name { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

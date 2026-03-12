using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class EmployeeType : BaseEntity
    {
        public string Name { get; set; }
        public decimal NoOfLeavesPerMonth { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

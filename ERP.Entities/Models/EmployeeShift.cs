using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class EmployeeShift : BaseEntity
    {
        [MaxLength(5)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Project : BaseEntity
    {
        [StringLength(5)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual List<AspNetUsers> Users { get; set; }
        public virtual List<Patient> Patients { get; set; }
        public virtual ICollection<ProjectStore> ProjectStore { get; set; } = new List<ProjectStore>();
    }
}

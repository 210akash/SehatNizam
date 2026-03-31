using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Clinical { get; set; } = false;
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual List<AspNetUsers> Users { get; set; }
    }
}

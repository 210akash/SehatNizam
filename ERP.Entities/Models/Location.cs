using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual List<Store> Stores { get; set; }

    }
}

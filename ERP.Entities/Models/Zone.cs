using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Zone : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }

        public long? RegionId { get; set; }
        public virtual Region Region { get; set; }

        public virtual ICollection<AspNetUsers> Salesmen { get; set; }
        public virtual ICollection<Area> Area { get; set; }
    }
}

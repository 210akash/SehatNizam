using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Area : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public long? ZoneId { get; set; }
        public virtual Zone Zone { get; set; }

        public virtual ICollection<Territory> Territory { get; set; }
    }
}

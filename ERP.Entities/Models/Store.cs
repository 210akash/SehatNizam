using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Store : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public long? LocationId { get; set; }
        public virtual Location Location { get; set; }
        public bool FixedAsset { get; set; }

        public virtual ICollection<CategoryStore> CategoryStores { get; set; } = new List<CategoryStore>();

        public virtual List<AspNetUsers> Users { get; set; }
    }
}

using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ShopType : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Shop> Shop { get; set; }
    }
}

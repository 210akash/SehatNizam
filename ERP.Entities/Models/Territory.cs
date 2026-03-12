using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Territory : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public string SaleModel { get; set; }

        public long? AreaId { get; set; }
        public virtual Area Area { get; set; }

        public virtual ICollection<Dealership> Dealership { get; set; }
        public virtual ICollection<Shop> Shop { get; set; }
        public virtual ICollection<Route> Route { get; set; }
    }
}

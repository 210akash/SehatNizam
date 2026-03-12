using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Route : BaseEntity
    {
        public string Name { get; set; }

        public long TerritoryId { get; set; }
        //public string VisitDay { get; set; }
        public virtual Territory Territory { get; set; }

        public ICollection<RouteShop> RouteShop { get; set; }
        //public ICollection<DSFRoute> DSFRoute { get; set; }
        public ICollection<ShopRouteFrequency> ShopRouteFrequency { get; set; }
    }
}

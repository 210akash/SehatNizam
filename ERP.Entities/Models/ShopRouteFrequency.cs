namespace ERP.Entities.Models
{
    public class ShopRouteFrequency : BaseEntity
    {
        public long ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public long RouteId { get; set; }
        public virtual Route Route { get; set; }


        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}

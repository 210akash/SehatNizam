namespace ERP.Entities.Models
{
    public class RouteShop : BaseEntity
    {
        public long? SequenceNo { get; set; }
        public long RouteId { get; set; }
        public virtual Route Route { get; set; }

        public long ShopId { get; set; }
        public virtual Shop Shop { get; set; }
    }
}

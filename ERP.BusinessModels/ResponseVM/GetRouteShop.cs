namespace ERP.BusinessModels.ResponseVM
{
    public class GetRouteShop
    {
        public long Id { get; set; }
        public long? SequenceNo { get; set; }
        public long RouteId { get; set; }
        public GetRoute Route { get; set; }

        public long ShopId { get; set; }
        public GetShop Shop { get; set; }

        public bool? IsActive { get; set; }
    }
}

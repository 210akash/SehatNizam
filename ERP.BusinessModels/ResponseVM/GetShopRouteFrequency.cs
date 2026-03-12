namespace ERP.BusinessModels.ResponseVM
{
    public class GetShopRouteFrequency
    {
        public long ShopId { get; set; }
        public GetShop Shop { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public long RouteId { get; set; }
        public GetRoute Route { get; set; }

        public bool IsActive { get; set; }
    }
}

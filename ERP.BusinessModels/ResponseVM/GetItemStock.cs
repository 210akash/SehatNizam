namespace ERP.BusinessModels.ResponseVM
{
    public class GetItemStock
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Volume { get; set; }
        public decimal QuantityInPack { get; set; }
        public string Image { get; set; }
        public int LeftQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int TransitQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public int RetailPrice { get; set; }
        public int TradePrice { get; set; }
        public int DistributorPrice { get; set; }
        public int DistributorPromo { get; set; }
        public bool IsActive { get; set; }

    }
}

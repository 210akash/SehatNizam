namespace ERP.BusinessModels.ResponseVM
{
    public class GetOrderItems
    {
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public long Id { get; set; }
        public int Quantity { get; set; }
        public int? ShippedQuantity { get; set; }
        public int? HoldQuantity { get; set; }
        public int? LeftQuantity { get; set; }
        public decimal? DistributorPrice { get; set; }
        public decimal? CustomDistributorPrice { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? CustomTradePrice { get; set; }
        public decimal? DistributorPromo { get; set; }
        public decimal? RetailPrice { get; set; }
        public long OrderId { get; set; }
        public GetOrder Order { get; set; }

        public long ItemId { get; set; }
        public GetItem Item { get; set; }

    }

    public class CreateOrderItems
    {
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public long Id { get; set; }
        public int Quantity { get; set; }
        public int? ShippedQuantity { get; set; }
        public int? HoldQuantity { get; set; }
        public int? LeftQuantity { get; set; }
        public decimal? DistributorPrice { get; set; }
        public decimal? CustomDistributorPrice { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? CustomTradePrice { get; set; }
        public decimal? DistributorPromo { get; set; }
        public decimal? RetailPrice { get; set; }
        public long OrderId { get; set; }
        public long ItemId { get; set; }
    }

    public class GetDealershipOrderItems
    {
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
        public decimal? DistributorPrice { get; set; }
        public decimal? CustomDistributorPrice { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? CustomTradePrice { get; set; }
        public decimal? DistributorPromo { get; set; }
        public decimal? RetailPrice { get; set; }
        public GetItem Item { get; set; }
    }
}

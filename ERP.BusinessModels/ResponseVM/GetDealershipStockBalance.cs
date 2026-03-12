namespace ERP.BusinessModels.ResponseVM
{
    public class GetDealershipStockBalance
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal QuantityIn { get; set; }
        public decimal QuantityOut { get; set; }
        public decimal Balance { get; set; }
    }
}

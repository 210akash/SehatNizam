namespace ERP.Entities.Models
{
    public class PriceGroupDetails : BaseEntity
    {
        public decimal RetailPrice { get; set; }
        public decimal TradePrice { get; set; }
        public decimal DistributorPrice { get; set; }
        public decimal DistributorPromo { get; set; }
        public decimal NetDistributorPrice { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public long? PriceGroupId { get; set; }
        public virtual PriceGroup PriceGroup { get; set; }
    }
}

using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class RetailOrderItems : BaseEntity
    {
        public long RetailOrderId { get; set; }
        public virtual RetailOrder RetailOrder { get; set; }

        public int Quantity { get; set; }
        public int? ShippedQuantity { get; set; }
        public decimal? DistributorPromo { get; set; }
        public decimal DistributorPrice { get; set; }
        public decimal? CustomDistributorPrice { get; set; }
        public decimal TradePrice { get; set; }
        public decimal? CustomTradePrice { get; set; }
        public decimal? RetailPrice { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}

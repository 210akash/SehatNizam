using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ShopOrderItems : BaseEntity
    {
        public long ShopOrderId { get; set; }
        public virtual ShopOrder ShopOrder { get; set; }

        public int Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public virtual List<ShopDispatchDetail> ShopDispatchDetails { get; set; }
    }
}
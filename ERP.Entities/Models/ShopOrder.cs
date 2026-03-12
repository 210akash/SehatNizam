using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ShopOrder : BaseEntity
    {
        public long? ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public string PaymentMode { get; set; }
        public decimal? Amount { get; set; }

        public long ShopOrderStatusId { get; set; }
        public virtual Status ShopOrderStatus { get; set; }
        public string Remarks { get; set; }

        public virtual ICollection<ShopOrderItems> ShopOrderItems { get; set; }
        public virtual ICollection<ShopDispatchDetail> ShopDispatchDetail { get; set; }
    }
}

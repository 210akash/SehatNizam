namespace ERP.Entities.Models
{
    public class ShopDispatchDetail : BaseEntity
    {
        public long ShopDispatchId { get; set; }
        public virtual ShopDispatch ShopDispatch { get; set; }

        public long ShopOrderItemId { get; set; }
        public virtual ShopOrderItems ShopOrderItem { get; set; }

        public long Quantity { get; set; }
    }
}
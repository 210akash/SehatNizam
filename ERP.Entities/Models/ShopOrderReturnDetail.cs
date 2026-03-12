namespace ERP.Entities.Models
{
    public class ShopOrderReturnDetail : BaseEntity
    {
        public long ShopOrderReturnId { get; set; }
        public virtual ShopOrderReturn ShopOrderReturn { get; set; }

        public long ShopOrderItemsId { get; set; }
        public virtual ShopOrderItems ShopOrderItems { get; set; }

        public decimal Quantity { get; set; }
    }
}

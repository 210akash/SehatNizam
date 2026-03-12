namespace ERP.Entities.Models
{
    public class RetailOrderReturnDetail : BaseEntity
    {
        public long RetailOrderReturnId { get; set; }
        public virtual RetailOrderReturn RetailOrderReturn { get; set; }

        public long RetailOrderItemsId { get; set; }
        public virtual RetailOrderItems RetailOrderItems { get; set; }

        public decimal Quantity { get; set; }
    }
}

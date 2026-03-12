namespace ERP.Entities.Models
{
    public class PurchaseReturnDetail : BaseEntity
    {
        public long PurchaseReturnId { get; set; }
        public virtual PurchaseReturn PurchaseReturn { get; set; }

        public long GRNDetailId { get; set; }
        public virtual GRNDetail GRNDetail { get; set; }

        public decimal Quantity { get; set; }
    }
}

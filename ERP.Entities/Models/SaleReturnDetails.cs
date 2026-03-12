namespace ERP.Entities.Models
{
    public class SaleReturnDetail : BaseEntity
    {
        public long SaleReturnId { get; set; }
        public virtual SaleReturn SaleReturn { get; set; }

        public long DispatchDetailId { get; set; }
        public virtual DispatchDetail DispatchDetail { get; set; }

        public decimal Quantity { get; set; }
    }
}

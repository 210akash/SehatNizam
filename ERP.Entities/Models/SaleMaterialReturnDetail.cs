namespace ERP.Entities.Models
{
    public class SaleMaterialReturnDetail : BaseEntity
    {
        public long SaleMaterialReturnId { get; set; }
        public virtual SaleMaterialReturn SaleMaterialReturn { get; set; }

        public long SaleMaterialDetailId { get; set; }
        public virtual SaleMaterialDetail SaleMaterialDetail { get; set; }

        public decimal Quantity { get; set; }
    }
}

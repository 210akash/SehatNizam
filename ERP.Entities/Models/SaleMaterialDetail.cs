namespace ERP.Entities.Models
{
    public class SaleMaterialDetail : BaseEntity
    {
        public long SaleMaterialId { get; set; }
        public virtual SaleMaterial SaleMaterial { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}

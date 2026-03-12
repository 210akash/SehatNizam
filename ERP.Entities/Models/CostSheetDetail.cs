namespace ERP.Entities.Models
{
    public class CostSheetDetail : BaseEntity
    {
        public long CostSheetId { get; set; }
        public virtual CostSheet CostSheet { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}

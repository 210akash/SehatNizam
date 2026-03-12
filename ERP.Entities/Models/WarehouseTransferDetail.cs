namespace ERP.Entities.Models
{
    public class WarehouseTransferDetail : BaseEntity
    {
        public long WarehouseTransferId { get; set; }
        public virtual WarehouseTransfer WarehouseTransfer { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public long? CostSheetId { get; set; }
        public virtual CostSheet CostSheet { get; set; }

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}

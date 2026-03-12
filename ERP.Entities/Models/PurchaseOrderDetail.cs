using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public class PurchaseOrderDetail : BaseEntity
    {
        public long PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        
        public long PurchaseDemandDetailId { get; set; }
        public virtual PurchaseDemandDetail PurchaseDemandDetail { get; set; }

        public long? ComparativeStatementVendorId { get; set; }
        public virtual ComparativeStatementVendor ComparativeStatementVendor { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal UnitRate { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal Value { get; set; }
        public decimal FED { get; set; }
        public decimal GST { get; set; }
        public string Description { get; set; }
    }
}

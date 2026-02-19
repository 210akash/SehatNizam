using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ComparativeStatementDetail : BaseEntity
    {
        public long ComparativeStatementId { get; set; }
        public virtual ComparativeStatement ComparativeStatement { get; set; }

        public long PurchaseDemandDetailId { get; set; }
        public virtual PurchaseDemandDetail PurchaseDemandDetail { get; set; }

        public virtual List<ComparativeStatementVendor> ComparativeStatementVendor { get; set; }
    }
}

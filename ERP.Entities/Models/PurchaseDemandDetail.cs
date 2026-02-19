using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class PurchaseDemandDetail : BaseEntity
    {
        public long PurchaseDemandId { get; set; }
        public virtual PurchaseDemand PurchaseDemand { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public decimal DemandQty { get; set; }
        public DateTime RequiredDate { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public string Description { get; set; }
         
        public virtual ComparativeStatementDetail ComparativeStatementDetail { get; set; }
    }
}

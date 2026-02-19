using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class IGP : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<IGPDetails> IGPDetails { get; set; }
    }
}

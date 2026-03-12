using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class PurchaseReturn : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long GRNId { get; set; }
        public virtual GRN GRN { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<PurchaseReturnDetail> PurchaseReturnDetail { get; set; }
    }
}

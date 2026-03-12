using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class ComparativeStatement : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long PurchaseDemandId { get; set; }
        public virtual PurchaseDemand PurchaseDemand { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }
        public virtual List<ComparativeStatementDetail> ComparativeStatementDetail { get; set; }
    }
}

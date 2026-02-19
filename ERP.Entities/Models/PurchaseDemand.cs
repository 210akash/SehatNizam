using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class PurchaseDemand : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }
  
        public string RequestNo { get; set; }
        public DateTime RequestDate { get; set; }

        public long? IndentTypeId { get; set; }
        public virtual IndentType IndentType { get; set; }

        public long PriorityId { get; set; }
        public virtual Priority Priority { get; set; }

        public long LocationId { get; set; }
        public virtual Location Location { get; set; }

        public long? StoreId { get; set; }
        public virtual Store Store { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }
        public virtual List<PurchaseDemandDetail> PurchaseDemandDetail { get; set; }
    }
}

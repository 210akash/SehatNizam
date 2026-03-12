using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Issuance : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }
        public DateTime Date { get; set; }

        public long IndentRequestId { get; set; }
        public virtual IndentRequest IndentRequest { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<IssuanceDetail> IssuanceDetail { get; set; }
    }
}

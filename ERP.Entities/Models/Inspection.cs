using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Inspection : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long IGPId { get; set; }
        public virtual IGP IGP { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<InspectionDetail> InspectionDetail { get; set; }
    }
}

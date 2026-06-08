using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ERP.Entities.Models
{
    public class RadiologyStudyResult : BaseEntity
    {
        public long RadiologyOrderId { get; set; }
        public virtual RadiologyOrder RadiologyOrder { get; set; }

        public Guid? PerformedById { get; set; }
        public virtual AspNetUsers? PerformedBy { get; set; }

        public Guid? ReportedById { get; set; }
        public virtual AspNetUsers? ReportedBy { get; set; }

        public DateTime PerformedDate { get; set; }
        public string ClinicalHistory { get; set; }

        public string Findings { get; set; }

        public string Impression { get; set; }

        public string Conclusion { get; set; }

        public virtual ICollection<RadiologyStudyImage> Images { get; set; }
            = new HashSet<RadiologyStudyImage>();
    }
}

using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRadiologyStudyResult
    {
        public long Id { get; set; }
        public long RadiologyOrderId { get; set; }
        public Guid? PerformedById { get; set; }
        public Guid? ReportedById { get; set; }
        public DateTime PerformedDate { get; set; }
        public string ClinicalHistory { get; set; }
        public string Findings { get; set; }
        public string Impression { get; set; }
        public string Conclusion { get; set; }
        public List<GetRadiologyStudyImage> Images { get; set; }
    }
}

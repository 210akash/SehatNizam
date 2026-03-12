using System;

namespace ERP.Entities.Models
{
    public class GRNDetail : BaseEntity
    {
        public long GRNId { get; set; }
        public virtual GRN GRN { get; set; }

        public long InspectionDetailId { get; set; }
        public virtual InspectionDetail InspectionDetail { get; set; }

        public long? SectionId { get; set; }
        public virtual Section Section { get; set; }

        public decimal Received { get; set; }

        public long? CostSheetId { get; set; }
        public virtual CostSheet CostSheet { get; set; }

        public string Refernace { get; set; }
        
        public DateTime ExpireDate { get; set; }
    }
}

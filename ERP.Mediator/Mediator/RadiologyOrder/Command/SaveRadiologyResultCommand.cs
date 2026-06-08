using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RadiologyOrder.Command
{
 
    public class SaveRadiologyStudyResultCommand : IRequest<long>
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
        public virtual ICollection<SaveRadiologyStudyImageCommand> Images { get; set; } = new HashSet<SaveRadiologyStudyImageCommand>();
    }

    public class SaveRadiologyStudyImageCommand
    {
        public long Id { get; set; }
        public long RadiologyStudyResultId { get; set; }
        public string ImageUrl { get; set; }
        public int SequenceNo { get; set; }
        public string Remarks { get; set; }
    }
}

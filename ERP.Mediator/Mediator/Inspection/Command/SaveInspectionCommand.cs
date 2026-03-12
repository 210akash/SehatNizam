using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Inspection.Command
{
    public class SaveInspectionCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long IGPId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SaveInspectionDetailCommand> InspectionDetail { get; set; }
    }

    public class SaveInspectionDetailCommand
    {
        public long Id { get; set; }
        public long InspectionId { get; set; }
        public long? RejectReasonId { get; set; }
        public decimal Rejected { get; set; }
        public string Remarks { get; set; }
        public long IGPDetailId { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.GRN.Command
{
    public class SaveGRNCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long InspectionId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SaveGRNDetailCommand> GRNDetail    { get; set; }
    }

    public class SaveGRNDetailCommand
    {
        public long Id { get; set; }
        public long GRNId { get; set; }
        public decimal Received { get; set; }
        public long InspectionDetailId { get; set; }
        public long? SectionId { get; set; }
        public long? CostSheetId { get; set; }
        public string Refernace { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}

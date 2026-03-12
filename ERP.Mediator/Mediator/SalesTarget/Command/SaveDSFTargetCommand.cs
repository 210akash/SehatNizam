using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SalesTarget.Command
{
    public class SaveDSFTargetCommand : IRequest<long>
    {
        public DateTime TargetMonth { get; set; }
        public long? ZoneId { get; set; }
        public long? TerritoryId { get; set; }

        public List<DSFTargetList> DSFTargetList { get; set; }
    }

    public class DSFTargetList
    {
        public long Id { get; set; }
        public Guid? DSFId { get; set; }
        public long Target { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SalesTarget.Command
{
    public class SaveTerritoryTargetCommand : IRequest<long>
    {
        public DateTime TargetMonth { get; set; }
        public long? ZoneId { get; set; }

        public List<TerritoryTargetList> TerritoriesTargetList { get; set; }
    }

    public class TerritoryTargetList
    {
        public long Id { get; set; }
        public long? TerritoryId { get; set; }
        public long Target { get; set; }
    }
}

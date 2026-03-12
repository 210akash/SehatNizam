using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Query
{
    public class GetTerritoryTargetsByZoneIdQuery : IRequest<List<GetSalesTarget>>
    {
        public GetTerritoryTargetsByZoneIdQuery(long ZoneId, DateTime TargetMonth)
        {
            this.ZoneId = ZoneId;
            this.TargetMonth = TargetMonth;
        }

        public long ZoneId { get; set; }
        public DateTime TargetMonth { get; set; }
    }
}
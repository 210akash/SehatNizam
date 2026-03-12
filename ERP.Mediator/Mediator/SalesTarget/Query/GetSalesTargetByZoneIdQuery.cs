using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetSalesTargetByZoneIdHandler;

namespace ERP.Mediator.Mediator.SalesTarget.Query
{
    public class GetSalesTargetByZoneIdQuery : IRequest<List<GroupedSalesTarget>>
    {
        public GetSalesTargetByZoneIdQuery(long ZoneId, DateTime TargetMonth)
        {
            this.ZoneId = ZoneId;
            this.TargetMonth = TargetMonth;
        }

        public long ZoneId { get; set; }
        public DateTime TargetMonth { get; set; }
    }
}
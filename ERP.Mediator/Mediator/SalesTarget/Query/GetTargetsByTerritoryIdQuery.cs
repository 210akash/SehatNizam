using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetTargetsByTerritoryIdHandler;

namespace ERP.Mediator.Mediator.SalesTarget.Query
{
    public class GetTargetsByTerritoryIdQuery : IRequest<GetTerritoryTarget>
    {
        public GetTargetsByTerritoryIdQuery(long TerritoryId, DateTime TargetMonth)
        {
            this.TerritoryId = TerritoryId;
            this.TargetMonth = TargetMonth;
        }

        public long TerritoryId { get; set; }
        public DateTime TargetMonth { get; set; }
    }
}
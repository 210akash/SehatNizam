using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class GetRoutesByDsfIdQuery : IRequest<List<GetRoute>>
    {
        public GetRoutesByDsfIdQuery(string DsfId, long TerritoryId)
        {
            this.DsfId = DsfId;
            this.TerritoryId = TerritoryId;
        }

        public string DsfId { get; set; }
        public long TerritoryId { get; set; }
    }
}
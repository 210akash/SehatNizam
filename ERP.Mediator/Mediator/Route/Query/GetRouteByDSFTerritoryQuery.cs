using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class GetRouteByDSFTerritoryQuery : IRequest<List<GetRoute>>
    {
        public GetRouteByDSFTerritoryQuery(string DsfId)
        {
            this.DsfId = DsfId;
        }

        public string DsfId { get; set; }
    }
}
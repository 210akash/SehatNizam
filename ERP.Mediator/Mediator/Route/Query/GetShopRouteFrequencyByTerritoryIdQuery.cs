using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class GetShopRouteFrequencyByTerritoryIdQuery : IRequest<List<GetShopRouteFrequency>>
    {
        public GetShopRouteFrequencyByTerritoryIdQuery(long TerritoryId)
        {
            this.TerritoryId = TerritoryId;
        }

        public long TerritoryId { get; set; }
    }
}
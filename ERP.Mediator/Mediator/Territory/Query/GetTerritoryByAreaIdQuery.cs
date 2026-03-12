using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Territory.Query
{
    public class GetTerritoryByAreaIdQuery : IRequest<List<GetTerritory>>
    {
        public GetTerritoryByAreaIdQuery(long AreaId)
        {
            this.AreaId = AreaId;
        }

        public long AreaId { get; set; }
    }
}
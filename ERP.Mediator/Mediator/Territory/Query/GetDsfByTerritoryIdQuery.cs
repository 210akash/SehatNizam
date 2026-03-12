using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Territory.Query
{
    public class GetDsfByTerritoryIdQuery : IRequest<List<GetUsers>>
    {
        public GetDsfByTerritoryIdQuery(long TerritoryId)
        {
            this.TerritoryId = TerritoryId;
        }

        public long TerritoryId { get; set; }
    }
}
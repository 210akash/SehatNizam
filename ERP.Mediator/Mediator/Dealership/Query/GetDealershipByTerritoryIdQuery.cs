using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetDealershipByTerritoryIdQuery : IRequest<List<GetDealership>>
    {
        public GetDealershipByTerritoryIdQuery(long TerritoryId)
        {
            this.TerritoryId = TerritoryId;
        }

        public long TerritoryId { get; set; }
    }
}
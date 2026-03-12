using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class GetShopsByTerritoryIdQuery : IRequest<List<GetShop>>
    {
        public GetShopsByTerritoryIdQuery(long TerritoryId)
        {
            this.TerritoryId = TerritoryId;
        }

        public long TerritoryId { get; set; }
    }
}
using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class SearchShopsByTerritoryIdQuery : IRequest<List<GetShopBasic>>
    {
        public SearchShopsByTerritoryIdQuery(long TerritoryId, string Param)
        {
            this.TerritoryId = TerritoryId;
            this.Param = Param;
        }

        public long TerritoryId { get; set; }
        public string Param { get; set; }
    }
}
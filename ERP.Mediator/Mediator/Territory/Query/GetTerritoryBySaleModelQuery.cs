using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Territory.Query
{
    public class GetTerritoryBySaleModelQuery : IRequest<List<GetTerritory>>
    {
        public GetTerritoryBySaleModelQuery(long AreaId, string SaleModel)
        {
            this.AreaId = AreaId;
            this.SaleModel = SaleModel;
        }

        public long AreaId { get; set; }
        public string SaleModel { get; set; }
    }
}
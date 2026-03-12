using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetDealershipByTerritorySaleModelQuery : IRequest<GetDealership>
    {
        public GetDealershipByTerritorySaleModelQuery(string SaleModel, long territoryId)
        {
            this.SaleModel = SaleModel;
            TerritoryId = territoryId;
        }

        public string SaleModel { get; set; }
        public long TerritoryId { get; set; }
    }
}
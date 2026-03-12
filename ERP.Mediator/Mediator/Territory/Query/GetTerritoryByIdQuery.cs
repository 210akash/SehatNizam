using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Query
{
    public class GetTerritoryByIdQuery : IRequest<GetTerritory>
    {
        public GetTerritoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
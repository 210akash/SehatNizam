using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Query
{
    public class GetUserTerritoryByIdQuery : IRequest<GetUserTerritory>
    {
        public GetUserTerritoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
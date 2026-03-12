using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Query
{
    public class GetUserTerritoryByNameQuery : IRequest<List<GetUserTerritory>>
    {
        public GetUserTerritoryByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
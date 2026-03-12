using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Query
{
    public class GetRoleByNameQuery : IRequest<List<GetRoles>>
    {
        public GetRoleByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
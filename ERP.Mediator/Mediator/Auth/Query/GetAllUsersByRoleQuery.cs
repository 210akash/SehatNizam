using MediatR;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAllUsersByRoleQuery : IRequest<List<GetUsers>>
    {
        public GetAllUsersByRoleQuery(string role)
        {
            this.role = role;
        }

        public string role { get; set; }
    }
}
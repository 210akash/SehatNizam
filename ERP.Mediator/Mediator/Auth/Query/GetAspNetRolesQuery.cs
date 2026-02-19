using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAspNetRolesQuery : IRequest<List<GetRoles>>
    {
        public GetAspNetRolesQuery()
        {

        }
    }
}

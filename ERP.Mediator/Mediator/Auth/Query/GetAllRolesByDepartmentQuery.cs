using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAllRolesByDepartmentQuery : IRequest<List<GetRoles>>
    {
        public GetAllRolesByDepartmentQuery(long departmentId)
        {
            this.departmentId = departmentId;
        }

        public long departmentId { get; set; }
    }
}

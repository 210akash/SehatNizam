using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Query
{
    public class GetDepartmentByNameQuery : IRequest<List<GetDepartment>>
    {
        public GetDepartmentByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
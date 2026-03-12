using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Query
{
    public class GetEmployeeLeaveGroupByNameQuery : IRequest<List<GetEmployeeLeaveGroup>>
    {
        public GetEmployeeLeaveGroupByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
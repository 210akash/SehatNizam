using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Query
{
    public class GetEmployeeLeaveByNameQuery : IRequest<List<GetEmployeeLeave>>
    {
        public GetEmployeeLeaveByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
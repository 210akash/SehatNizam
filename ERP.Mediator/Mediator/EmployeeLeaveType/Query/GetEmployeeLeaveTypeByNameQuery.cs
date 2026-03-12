using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Query
{
    public class GetEmployeeLeaveTypeByNameQuery : IRequest<List<GetEmployeeLeaveType>>
    {
        public GetEmployeeLeaveTypeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
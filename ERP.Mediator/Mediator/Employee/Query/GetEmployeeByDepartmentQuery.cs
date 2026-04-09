using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Employee.Query
{
    public class GetEmployeeByDepartment : IRequest<List<GetEmployee>>
    {
        public GetEmployeeByDepartment(long DepartmentId)
        {
            this.DepartmentId = DepartmentId;
        }

        public long DepartmentId { get; set; }
    }
}
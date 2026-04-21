using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query
{
    public class GetAllEmployeeSalariesQuery : IRequest<IEnumerable<GetEmployeeSalary>>
    {
        public long? EmployeeId { get; set; }
    }
}

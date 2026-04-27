using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query
{
    /// <summary>
    /// Gets the latest active salary records for an employee as of a specific date
    /// </summary>
    public class GetEmployeeSalaryByEmployeeIdQuery : IRequest<IEnumerable<GetEmployeeSalary>>
    {
        public string EmployeeId { get; set; }
    }
}

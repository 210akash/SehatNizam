using System;
using System.Collections.Generic;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command
{

    public class SaveEmployeeSalaryCommand : IRequest<Tuple<long, string>>
    {
        public Guid EmployeeId { get; set; }

        public List<EmployeeSalaryCommand> EmployeeSalary { get; set; }
    }


    public class EmployeeSalaryCommand : IRequest<int>
    {
        public long Id { get; set; }
        public long SalaryHeadId { get; set; }
        public decimal Amount { get; set; }
        public DateTime EffectiveFrom { get; set; } = DateTime.Now;
    }
}

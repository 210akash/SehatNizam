using System;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command
{
    public class SaveEmployeeSalaryCommand : IRequest<int>
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long SalaryHeadId { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveFrom { get; set; } = DateTime.Now;
    }
}

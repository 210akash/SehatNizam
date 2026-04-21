using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command
{
    public class DeleteEmployeeSalaryCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteEmployeeSalaryCommand(long id)
        {
            Id = id;
        }
    }
}

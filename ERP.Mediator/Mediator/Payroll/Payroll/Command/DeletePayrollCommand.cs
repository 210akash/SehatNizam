using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Command
{
    public class DeletePayrollCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeletePayrollCommand(long id)
        {
            Id = id;
        }
    }
}

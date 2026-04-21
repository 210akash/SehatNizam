using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Command
{
    public class DeleteSalaryHeadCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteSalaryHeadCommand(long id)
        {
            Id = id;
        }
    }
}

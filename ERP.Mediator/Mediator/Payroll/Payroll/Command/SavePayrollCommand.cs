using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Command
{
    public class SavePayrollCommand : IRequest<int>
    {
        public long Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public PayrollStatus Status { get; set; }
    }
}

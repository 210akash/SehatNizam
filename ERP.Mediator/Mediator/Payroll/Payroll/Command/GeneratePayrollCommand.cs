using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Command
{
    /// <summary>
    /// Command to generate payroll for a specific month and year
    /// </summary>
    public class GeneratePayrollCommand : IRequest<int>
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}

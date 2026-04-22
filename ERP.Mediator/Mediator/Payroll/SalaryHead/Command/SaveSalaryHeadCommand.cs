using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Command
{
    public class SaveSalaryHeadCommand : IRequest<int>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}

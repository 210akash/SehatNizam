using MediatR;

namespace ERP.Mediator.Mediator.EmployeeOvertimeRate.Command
{
    public class SaveEmployeeOvertimeRateCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Rate { get; set; }
    }
}

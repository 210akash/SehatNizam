using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Command
{
    public class SaveEmployeeShiftCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}

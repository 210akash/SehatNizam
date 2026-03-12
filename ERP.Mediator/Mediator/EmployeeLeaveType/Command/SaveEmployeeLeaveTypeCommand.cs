using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Command
{
    public class SaveEmployeeLeaveTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Command
{
    public class SaveEmployeeLeaveGroupCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

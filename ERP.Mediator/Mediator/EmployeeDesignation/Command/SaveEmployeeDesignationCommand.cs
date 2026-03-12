using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Command
{
    public class SaveEmployeeDesignationCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

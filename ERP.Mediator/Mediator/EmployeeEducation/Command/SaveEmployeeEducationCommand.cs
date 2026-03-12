using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Command
{
    public class SaveEmployeeEducationCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

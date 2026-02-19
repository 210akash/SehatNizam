using MediatR;

namespace ERP.Mediator.Mediator.Department.Command
{
    public class SaveDepartmentCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Command
{
    public class SaveEmployeeDocumentTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

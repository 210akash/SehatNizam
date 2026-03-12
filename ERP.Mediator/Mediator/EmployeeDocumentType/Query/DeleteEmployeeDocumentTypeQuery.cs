using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Query
{
    public class DeleteEmployeeDocumentTypeQuery : IRequest<bool>
    {
        public DeleteEmployeeDocumentTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
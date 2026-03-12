using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Query
{
    public class DeleteInspectionQuery : IRequest<bool>
    {
        public DeleteInspectionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Query
{
    public class DeleteLabOrderQuery : IRequest<bool>
    {
        public DeleteLabOrderQuery(long id) { Id = id; }
        public long Id { get; set; }
    }
}

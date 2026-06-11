using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Rack.Query
{
    public class DeleteRackQuery : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteRackQuery(long id) { Id = id; }
    }
}

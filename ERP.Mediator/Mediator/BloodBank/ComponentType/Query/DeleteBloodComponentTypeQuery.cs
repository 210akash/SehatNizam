using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.ComponentType.Query
{
    public class DeleteBloodComponentTypeQuery : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteBloodComponentTypeQuery(long id) { Id = id; }
    }
}

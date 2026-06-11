using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Fridge.Query
{
    public class DeleteFridgeQuery : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteFridgeQuery(long id) { Id = id; }
    }
}

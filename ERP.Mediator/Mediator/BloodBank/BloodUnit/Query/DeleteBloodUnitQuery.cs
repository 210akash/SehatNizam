using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Query
{
    public class DeleteBloodUnitQuery : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteBloodUnitQuery(long id) { Id = id; }
    }
}

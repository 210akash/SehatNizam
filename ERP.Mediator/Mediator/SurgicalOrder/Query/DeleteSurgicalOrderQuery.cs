using MediatR;

namespace ERP.Mediator.Mediator.SurgicalOrder.Query
{
    public class DeleteSurgicalOrderQuery : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteSurgicalOrderQuery(long id)
        {
            Id = id;
        }
    }
}

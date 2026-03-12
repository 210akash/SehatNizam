using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Query
{
    public class DeleteOrderQuery : IRequest<long>
    {
        public DeleteOrderQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
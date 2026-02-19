using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class DeleteItemQuery : IRequest<bool>
    {
        public DeleteItemQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
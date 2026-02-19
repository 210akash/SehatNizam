using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Query
{
    public class DeleteItemTypeQuery : IRequest<bool>
    {
        public DeleteItemTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
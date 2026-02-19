using MediatR;

namespace ERP.Mediator.Mediator.Store.Query
{
    public class DeleteStoreQuery : IRequest<bool>
    {
        public DeleteStoreQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
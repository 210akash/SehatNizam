using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class DeleteDispatchQuery : IRequest<bool>
    {
        public DeleteDispatchQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
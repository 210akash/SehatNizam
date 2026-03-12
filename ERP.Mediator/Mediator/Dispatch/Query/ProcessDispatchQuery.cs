using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class ProcessDispatchQuery : IRequest<bool>
    {
        public ProcessDispatchQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class ReceiveDispatchOrderQuery : IRequest<bool>
    {
        public ReceiveDispatchOrderQuery(long DispatchOrderId)
        {
            this.DispatchOrderId = DispatchOrderId;
        }

        public long DispatchOrderId { get; set; }
    }
}
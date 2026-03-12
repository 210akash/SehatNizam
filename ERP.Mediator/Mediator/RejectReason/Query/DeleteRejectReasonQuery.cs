using MediatR;

namespace ERP.Mediator.Mediator.RejectReason.Query
{
    public class DeleteRejectReasonQuery : IRequest<bool>
    {
        public DeleteRejectReasonQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class ApproveTransactionQuery : IRequest<bool>
    {
        public ApproveTransactionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
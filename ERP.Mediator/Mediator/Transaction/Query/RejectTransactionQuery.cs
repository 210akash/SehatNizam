using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class RejectTransactionQuery : IRequest<bool>
    {
        public RejectTransactionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
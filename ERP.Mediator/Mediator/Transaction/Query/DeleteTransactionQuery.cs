using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class DeleteTransactionQuery : IRequest<bool>
    {
        public DeleteTransactionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
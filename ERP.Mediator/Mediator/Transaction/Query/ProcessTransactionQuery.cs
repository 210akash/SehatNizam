using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class ProcessTransactionQuery : IRequest<bool>
    {
        public ProcessTransactionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
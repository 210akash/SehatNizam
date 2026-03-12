using MediatR;

namespace ERP.Mediator.Mediator.Ledger.Query
{
    public class GetCustomerBalanceQuery : IRequest<decimal>
    {
        public GetCustomerBalanceQuery(long CustomerId)
        {
            this.CustomerId = CustomerId;
        }

        public long CustomerId { get; set; }
    }
}
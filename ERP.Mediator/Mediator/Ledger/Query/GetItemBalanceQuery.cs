using MediatR;

namespace ERP.Mediator.Mediator.Ledger.Query
{
    public class GetItemBalanceQuery : IRequest<decimal>
    {
        public GetItemBalanceQuery(long ItemId)
        {
            this.ItemId = ItemId;
        }

        public long ItemId { get; set; }
    }
}
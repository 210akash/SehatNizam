using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class ProcessPurchaseDemandQuery : IRequest<bool>
    {
        public ProcessPurchaseDemandQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
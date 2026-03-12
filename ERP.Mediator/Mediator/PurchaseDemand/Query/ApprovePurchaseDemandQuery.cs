using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class ApprovePurchaseDemandQuery : IRequest<bool>
    {
        public ApprovePurchaseDemandQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
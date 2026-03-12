using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class DeletePurchaseDemandQuery : IRequest<bool>
    {
        public DeletePurchaseDemandQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
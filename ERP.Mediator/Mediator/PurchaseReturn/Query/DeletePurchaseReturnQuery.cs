using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Query
{
    public class DeletePurchaseReturnQuery : IRequest<bool>
    {
        public DeletePurchaseReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
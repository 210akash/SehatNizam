using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class DeletePurchaseOrderQuery : IRequest<bool>
    {
        public DeletePurchaseOrderQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
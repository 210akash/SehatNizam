using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class ProcessPurchaseOrderQuery : IRequest<bool>
    {
        public ProcessPurchaseOrderQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
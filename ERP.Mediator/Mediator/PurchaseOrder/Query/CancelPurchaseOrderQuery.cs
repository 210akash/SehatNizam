using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class CancelPurchaseOrderQuery : IRequest<bool>
    {
        public CancelPurchaseOrderQuery(long Id,string StatusRemarks)
        {
            this.Id = Id;
            this.StatusRemarks = StatusRemarks;
        }

        public long Id { get; set; }
        public string StatusRemarks { get; set; }
    }
}
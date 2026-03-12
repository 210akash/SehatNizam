using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class RejectPurchaseOrderQuery : IRequest<bool>
    {
        public RejectPurchaseOrderQuery(long Id,string StatusRemarks)
        {
            this.Id = Id;
            this.StatusRemarks = StatusRemarks;
        }

        public long Id { get; set; }
        public string StatusRemarks { get; set; }
    }
}
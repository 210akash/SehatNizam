using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IGP.Command
{
    public class SaveIGPCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long PurchaseOrderId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SaveIGPDetailsCommand> IGPDetails { get; set; }
    }

    public class SaveIGPDetailsCommand
    {
        public long Id { get; set; }
        public long IGPId { get; set; }
        public decimal Received { get; set; }
        public long PurchaseOrderDetailId { get; set; }
    }
}

using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseReturn.Command
{
    public class SavePurchaseReturnCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long GRNId { get; set; }
        public long? ProjectId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SavePurchaseReturnDetailCommand> PurchaseReturnDetail { get; set; }
    }

    public class SavePurchaseReturnDetailCommand
    {
        public long Id { get; set; }
        public long PurchaseReturnId { get; set; }
        public decimal Quantity { get; set; }
        public long GRNDetailId { get; set; }
    }
}

using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Command
{
    public class SaveRetailOrderReturnCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long RetailOrderId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SaveRetailOrderReturnDetailCommand> RetailOrderReturnDetail { get; set; }
    }

    public class SaveRetailOrderReturnDetailCommand
    {
        public long Id { get; set; }
        public long RetailOrderReturnId { get; set; }
        public long RetailOrderItemsId { get; set; }
        public decimal Quantity { get; set; }
    }
}

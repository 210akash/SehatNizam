using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SaleReturn.Command
{
    public class SaveSaleReturnCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long DispatchOrderId { get; set; }
        public long StatusId { get; set; }
        public long ProjectId { get; set; }
        public string Remarks { get; set; }
        public List<SaveSaleReturnDetailCommand> SaleReturnDetail { get; set; }
    }

    public class SaveSaleReturnDetailCommand
    {
        public long Id { get; set; }
        public long SaleReturnId { get; set; }
        public decimal Quantity { get; set; }
        public long DispatchDetailId { get; set; }
    }
}

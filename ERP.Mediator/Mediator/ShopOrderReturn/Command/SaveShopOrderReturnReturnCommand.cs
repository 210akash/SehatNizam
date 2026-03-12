using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Command
{
    public class SaveShopOrderReturnCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long OrderId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SaveShopOrderReturnDetailCommand> ShopOrderReturnDetail { get; set; }
    }

    public class SaveShopOrderReturnDetailCommand
    {
        public long Id { get; set; }
        public long ShopOrderReturnId { get; set; }
        public decimal Quantity { get; set; }
        public long OrderItemsId { get; set; }
    }
}

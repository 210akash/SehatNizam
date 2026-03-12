using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopDispatch.Command
{
    public class SaveShopDispatchCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public long ShopOrderId { get; set; }
        public string Remarks { get; set; }
        public string VehicleNo { get; set; }
        public List<SaveShopDispatchDetailCommand> ShopDispatchDetail { get; set; }
    }

    public class SaveShopDispatchDetailCommand
    {
        public long Id { get; set; }
        public long ShopDispatchId { get; set; }
        public long ShopOrderItemId { get; set; }
        public long Quantity { get; set; }
    }
}

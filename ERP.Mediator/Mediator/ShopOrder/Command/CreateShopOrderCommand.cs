using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopOrder.Command
{
    public class CreateShopOrderCommand : IRequest<long>
    {
        public long? Id { get; set; }
        public long? ShopId { get; set; }
        public string PaymentMode { get; set; }
        public decimal? Amount { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? ModifiedById { get; set; }
        public List<CreateShopOrderItems> ShopOrderItemsList { get; set; }
    }

    public class CreateShopOrderItems
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public long ItemId { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopOrder.Command
{
    public class CreateShopOrderByDealershipCommand : IRequest<long>
    {
        public long? Id { get; set; }
        public long? ShopId { get; set; }
        public long DealershipId { get; set; }
        public string PaymentMode { get; set; }
        public decimal? Amount { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime AppDateTime { get; set; }
        public List<CreateShopOrderItems> ShopOrderItemsList { get; set; }
    }
}

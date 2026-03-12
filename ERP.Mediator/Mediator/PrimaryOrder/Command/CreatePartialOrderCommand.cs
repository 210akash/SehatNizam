using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PrimaryOrder.Command
{
    public class CreatePartialOrderCommand : IRequest<long>
    {
        public long OrderId { get; set; }
        public string ImageName { get; set; }
        public long? DealershipId { get; set; }
        public string DealershipAddress { get; set; }
        public List<PartialOrderItemsCommand> PartialOrderItemsCommand { get; set; }
    }

    public class PartialOrderItemsCommand
    {
        public long ItemId { get; set; }
        public long? Quantity { get; set; }
        public long? ShippedQuantity { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? CustomDistributorPrice { get; set; }
        public decimal? DistributorPrice { get; set; }
    }
}

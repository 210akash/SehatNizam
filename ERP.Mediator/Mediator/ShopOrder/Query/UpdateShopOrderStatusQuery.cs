using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class UpdateShopOrderStatusQuery : IRequest<long>
    {
        public long ShopOrderId { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public string Comments { get; set; }
        public string TransactionId { get; set; }
    }
}
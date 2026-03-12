using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Query
{
    public class UpdateOrderStatusQuery : IRequest<long>
    {
        public long OrderId { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public string Comments { get; set; }
        public string TransactionId { get; set; }
    }
}
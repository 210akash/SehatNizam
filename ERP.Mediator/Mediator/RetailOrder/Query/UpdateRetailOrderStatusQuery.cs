using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Query
{
    public class UpdateRetailOrderStatusQuery : IRequest<long>
    {
        public long RetailOrderId { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public string Comments { get; set; }
    }
}
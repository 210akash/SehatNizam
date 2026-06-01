using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Query
{
    public class ConfirmLabOrderCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public long Discount { get; set; }
        public long PaymentModeId { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Query
{
    public class ConfirmRadiologyOrderCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public long Discount { get; set; }
        public long PaymentModeId { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Command
{
    public class SavePaymentModeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}

using MediatR;

namespace ERP.Mediator.Mediator.AdvancePayments.Command
{
    public class SaveAdvancePaymentsCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public long PaymentModeId { get; set; }
        public long PaymentStatusId { get; set; }
    }
}

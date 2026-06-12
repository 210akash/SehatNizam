using MediatR;

namespace ERP.Mediator.Mediator.AppointmentPayments.Command
{
    public class SaveAppointmentPaymentCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long ServiceId { get; set; }
        public decimal VisitFee { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPayable { get; set; }
        public long PaymentModeId { get; set; }
        public long PaymentStatusId { get; set; }
    }
}

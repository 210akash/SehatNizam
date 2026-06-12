using System.Collections.Generic;
using MediatR;

namespace ERP.Mediator.Mediator.AppointmentPayments.Command
{
    public class ApproveAppointmentPaymentItem
    {
        public long Id { get; set; }
        public decimal Discount { get; set; }
    }

    public class ApproveAppointmentPaymentsCommand : IRequest<long>
    {
        public long AppointmentId { get; set; }
        public long PaymentModeId { get; set; }
        public List<ApproveAppointmentPaymentItem> Payments { get; set; } = new();
    }
}

using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAdvancePayment
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public GetAppointment Appointment { get; set; }
        public decimal Amount { get; set; } = 0m;
        public GetPaymentMode PaymentMode { get; set; }
        public long PaymentModeId { get; set; }
        public DateTime PaymentDate { get; set; }
        public GetStatus PaymentStatus { get; set; }
        public long PaymentStatusId { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
        public Guid CreatedById { get; set; }
    }
}

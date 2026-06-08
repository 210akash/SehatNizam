using System;
namespace ERP.Entities.Models
{
    public class AdvancePayment : BaseEntity
    {
        public long AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public decimal Amount { get; set; } = 0m;
        public PaymentMode PaymentMode { get; set; }
        public long PaymentModeId { get; set; }
        public DateTime PaymentDate { get; set; }
        public Status PaymentStatus { get; set; }
        public long PaymentStatusId { get; set; }
    }
}

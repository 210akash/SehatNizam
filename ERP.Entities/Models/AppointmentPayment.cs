using System;
namespace ERP.Entities.Models
{
    public class AppointmentPayment : BaseEntity
    {
        public long AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public decimal VisitFee { get; set; } = 0m;
        public decimal Discount { get; set; } = 0m;
        public decimal TotalPayable { get; set; } = 0m;
        public PaymentMode PaymentMode { get; set; }
        public long PaymentModeId { get; set; }
        public long ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime PaymentDate { get; set; }
        public Status PaymentStatus { get; set; }
        public long PaymentStatusId { get; set; }
    }
}

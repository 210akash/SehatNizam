using ERP.Entities.Models;
using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAppointmentPayment
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public GetAppointment Appointment { get; set; }
        public decimal VisitFee { get; set; } = 0m;
        public decimal Discount { get; set; } = 0m;
        public decimal TotalPayable { get; set; } = 0m;
        public GetPaymentMode PaymentMode { get; set; }
        public long PaymentModeId { get; set; }
        public DateTime PaymentDate { get; set; }
        public GetStatus PaymentStatus { get; set; }
        public long PaymentStatusId { get; set; }
    }
}

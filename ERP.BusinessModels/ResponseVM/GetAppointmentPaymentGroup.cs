using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAppointmentPaymentGroup
    {
        public long AppointmentId { get; set; }
        public GetAppointment Appointment { get; set; }
        public int PendingPaymentCount { get; set; }
        public int ApprovedPaymentCount { get; set; }
        public decimal PendingGrandTotal { get; set; }
        public decimal ApprovedGrandTotal { get; set; }
        public decimal TotalVisitFee { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime? LastCreatedDate { get; set; }
    }
}

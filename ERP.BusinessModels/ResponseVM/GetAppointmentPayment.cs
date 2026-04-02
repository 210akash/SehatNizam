namespace ERP.BusinessModels.ResponseVM
{
    public class GetAppointmentPayment
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
    }
}

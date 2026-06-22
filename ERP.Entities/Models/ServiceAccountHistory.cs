namespace ERP.Entities.Models
{
    public class ServiceAccountHistory : BaseEntity
    {
        public long ServiceAccountId { get; set; }
        public ServiceAccount ServiceAccount { get; set; }
        public long ServiceId { get; set; }
        public ServiceAccountType AccountType { get; set; }
        public long? OldDebitAccountId { get; set; }
        public long? OldCreditAccountId { get; set; }
        public long? NewDebitAccountId { get; set; }
        public long? NewCreditAccountId { get; set; }
        public string Reason { get; set; }
    }
}

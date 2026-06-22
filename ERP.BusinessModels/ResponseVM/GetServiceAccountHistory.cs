
namespace ERP.BusinessModels.ResponseVM
{
    public class GetServiceAccountHistory
    {
        public long Id { get; set; }
        public long ServiceAccountId { get; set; }
        public GetServiceAccount ServiceAccount { get; set; }
        public long? OldDebitAccountId { get; set; }
        public long? OldCreditAccountId { get; set; }
        public long? NewDebitAccountId { get; set; }
        public long? NewCreditAccountId { get; set; }
        public string Reason { get; set; }
    }
}

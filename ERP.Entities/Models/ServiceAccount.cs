using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ServiceAccount : BaseEntity
    {
        public long ProjectId { get; set; }
        public Project Project { get; set; }
        public long ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public long PaymentModeId { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public long DebitAccountId { get; set; }
        public virtual Account DebitAccount { get; set; }
        public long CreditAccountId { get; set; }
        public virtual Account CreditAccount { get; set; }
        public ServiceAccountType AccountType { get; set; }
        public virtual List<ServiceAccountHistory> ServiceAccountHistory { get; set; }
    }

    public enum ServiceAccountType
    {
        Payable = 1,
        Discount = 2
    }
}

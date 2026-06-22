
using ERP.Entities.Models;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetServiceAccount
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public GetProject Project { get; set; }
        public long ServiceId { get; set; }
        public GetService Service { get; set; }
        public long DebitAccountId { get; set; }
        public virtual GetAccount DebitAccount { get; set; }
        public long CreditAccountId { get; set; }
        public virtual GetAccount CreditAccount { get; set; }
        public ServiceAccountType AccountType { get; set; }
    }
}

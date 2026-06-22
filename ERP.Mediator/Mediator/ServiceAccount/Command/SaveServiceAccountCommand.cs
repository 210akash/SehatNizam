using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ServiceAccount.Command
{
    public class SaveServiceAccountCommand : IRequest<int>
    {
        public long ServiceId { get; set; }
        public List<SaveServiceAccountItem> ServiceAccounts { get; set; }
    }

    public class SaveServiceAccountItem
    {
        public long ProjectId { get; set; }
        public ServiceAccountType AccountType { get; set; }
        public long DebitAccountId { get; set; }
        public long CreditAccountId { get; set; }
    }
}

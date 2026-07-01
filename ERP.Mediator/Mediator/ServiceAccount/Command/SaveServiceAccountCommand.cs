using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ServiceAccount.Command
{
    public class SaveServiceAccountCommand : IRequest<int>
    {
        public long ServiceTypeId { get; set; }
        public List<SaveServiceAccountItem> ServiceAccounts { get; set; }  // flat list
    }

    public class SaveServiceAccountItem
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public long PaymentModeId { get; set; }
        public ServiceAccountType AccountType { get; set; }
        public long DebitAccountId { get; set; }
        public long CreditAccountId { get; set; }
    }
}
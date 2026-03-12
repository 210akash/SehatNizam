using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.AuditReview.Command
{
    public class SaveAuditReviewCommand : IRequest<long>
    {
        public long OrderId { get; set; }
        public string Bank { get; set; }
        public string TransactionId { get; set; }
        public bool IsTransactionLedgerEntry { get; set; }
        public string Description { get; set; }
        public int? Amount { get; set; }
    }
}

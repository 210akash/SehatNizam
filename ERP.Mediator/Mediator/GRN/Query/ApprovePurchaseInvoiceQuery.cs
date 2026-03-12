using MediatR;
using System;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class ApprovePurchaseInvoiceQuery : IRequest<Tuple<long, string>>
    {
        public ApprovePurchaseInvoiceQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using MediatR;
using System;

namespace ERP.Mediator.Mediator.PurchaseReturn.Query
{
    public class ApprovePurchaseReturnQuery : IRequest<Tuple<long, string>>
    {
        public ApprovePurchaseReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
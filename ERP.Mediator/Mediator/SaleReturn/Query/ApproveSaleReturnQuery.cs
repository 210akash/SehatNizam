using MediatR;
using System;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class ApproveSaleReturnQuery : IRequest<Tuple<long, string>>
    {
        public ApproveSaleReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
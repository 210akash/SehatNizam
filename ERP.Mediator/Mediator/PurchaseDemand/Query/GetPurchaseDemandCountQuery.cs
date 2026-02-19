using System;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class GetPurchaseDemandCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
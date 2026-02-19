using System;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class GetPurchaseOrderCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
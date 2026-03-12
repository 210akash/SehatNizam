using System;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Query
{
    public class GetPurchaseReturnCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public long VendorId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
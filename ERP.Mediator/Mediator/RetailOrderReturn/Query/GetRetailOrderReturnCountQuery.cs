using System;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Query
{
    public class GetRetailOrderReturnCountQuery : IRequest<Tuple<long, long>>
    {
        public string Code { get; set; }
        public string RetailOrderId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
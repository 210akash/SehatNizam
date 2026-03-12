using System;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetDispatchCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public long? DealershipId { get; set; }
        public string OrderId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
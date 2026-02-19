using System;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Query
{
    public class GetIndentRequestCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
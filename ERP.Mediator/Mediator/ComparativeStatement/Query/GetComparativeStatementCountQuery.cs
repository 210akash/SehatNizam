using System;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class GetComparativeStatementCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
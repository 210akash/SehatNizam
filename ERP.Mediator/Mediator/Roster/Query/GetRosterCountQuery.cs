using System;
using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class GetRosterCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public long? ItemId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
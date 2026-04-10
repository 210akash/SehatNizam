using System;
using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class GetRosterCountQuery : IRequest<Tuple<long, long, long>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long? DepartmentId { get; set; }
        public long StatusId { get; set; }
    }
}
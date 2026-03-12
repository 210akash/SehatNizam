using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Area.Query
{
    public class GetAreaByZoneIdQuery : IRequest<List<GetArea>>
    {
        public GetAreaByZoneIdQuery(long ZoneId)
        {
            this.ZoneId = ZoneId;
        }

        public long ZoneId { get; set; }
    }
}
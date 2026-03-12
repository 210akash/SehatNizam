using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Zone.Query
{
    public class GetZoneByRegionIdQuery : IRequest<List<GetZone>>
    {
        public GetZoneByRegionIdQuery(long RegionId)
        {
            this.RegionId = RegionId;
        }

        public long RegionId { get; set; }
    }
}
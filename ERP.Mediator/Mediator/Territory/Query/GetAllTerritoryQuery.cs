using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Query
{
    public class GetAllTerritoryQuery : IRequest<Tuple<IEnumerable<GetTerritory>, long>>
    {
        public long RegionId { get; set; }
        public long ZoneId { get; set; }
        public long AreaId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
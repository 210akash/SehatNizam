using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.DSF.Query
{
    public class GetAllDSFQuery : IRequest<Tuple<IEnumerable<GetUsers>, long>>
    {
        public long RegionId { get; set; }
        public long ZoneId { get; set; }
        public long AreaId { get; set; }
        public long TerritoryId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
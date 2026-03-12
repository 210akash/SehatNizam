using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetAllDealershipQuery : IRequest<Tuple<IEnumerable<GetDealership>, long>>
    {
        public long RegionId { get; set; }
        public long ZoneId { get; set; }
        public long AreaId { get; set; }
        public long TerritoryId { get; set; }
        public long DealershipTypeId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
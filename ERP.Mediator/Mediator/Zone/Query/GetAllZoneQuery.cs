using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Query
{
    public class GetAllZoneQuery : IRequest<Tuple<IEnumerable<GetZone>, long>>
    {
        public long? RegionId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
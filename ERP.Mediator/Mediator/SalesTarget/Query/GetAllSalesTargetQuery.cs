using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Query
{
    public class GetAllSalesTargetQuery : IRequest<Tuple<IEnumerable<GetSalesTarget>, long>>
    {
        public long RegionId { get; set; }
        public long ZoneId { get; set; }
        public long AreaId { get; set; }
        public long TerritoryId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
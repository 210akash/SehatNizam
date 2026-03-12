using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Query
{
    public class GetAllAreaQuery : IRequest<Tuple<IEnumerable<GetArea>, long>>
    {
        public long? RegionId { get; set; }
        public long? ZoneId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
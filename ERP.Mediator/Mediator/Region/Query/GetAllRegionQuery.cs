using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Query
{
    public class GetAllRegionQuery : IRequest<Tuple<IEnumerable<GetRegion>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
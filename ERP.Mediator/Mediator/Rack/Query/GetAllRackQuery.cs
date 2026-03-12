using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Query
{
    public class GetAllRackQuery : IRequest<Tuple<IEnumerable<GetRack>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
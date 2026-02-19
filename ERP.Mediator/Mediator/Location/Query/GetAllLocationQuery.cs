using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Query
{
    public class GetAllLocationQuery : IRequest<Tuple<IEnumerable<GetLocation>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
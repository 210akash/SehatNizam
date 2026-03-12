using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Row.Query
{
    public class GetAllRowQuery : IRequest<Tuple<IEnumerable<GetRow>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
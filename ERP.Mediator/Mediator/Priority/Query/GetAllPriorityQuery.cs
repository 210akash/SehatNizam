using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Query
{
    public class GetAllPriorityQuery : IRequest<Tuple<IEnumerable<GetPriority>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
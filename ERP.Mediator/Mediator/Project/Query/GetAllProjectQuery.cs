using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Query
{
    public class GetAllProjectQuery : IRequest<Tuple<IEnumerable<GetProject>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
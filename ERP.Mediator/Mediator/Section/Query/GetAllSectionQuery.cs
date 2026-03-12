using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Query
{
    public class GetAllSectionQuery : IRequest<Tuple<IEnumerable<GetSection>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
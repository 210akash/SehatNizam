using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Query
{
    public class GetAllIndentTypeQuery : IRequest<Tuple<IEnumerable<GetIndentType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
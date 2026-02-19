using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Query
{
    public class GetAllCategoryQuery : IRequest<Tuple<IEnumerable<GetCategory>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
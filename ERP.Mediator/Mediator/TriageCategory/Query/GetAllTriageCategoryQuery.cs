using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.TriageCategory.Query
{
    public class GetAllTriageCategoryQuery : IRequest<Tuple<IEnumerable<GetTriageCategory>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
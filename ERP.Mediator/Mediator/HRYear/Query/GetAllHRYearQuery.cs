using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.HRYear.Query
{
    public class GetAllHRYearQuery : IRequest<Tuple<IEnumerable<GetHRYear>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
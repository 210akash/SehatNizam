using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.VisitType.Query
{
    public class GetAllVisitTypeQuery : IRequest<Tuple<IEnumerable<GetVisitType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
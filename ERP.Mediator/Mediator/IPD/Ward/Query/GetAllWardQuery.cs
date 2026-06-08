using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Ward.Query
{
    public class GetAllWardQuery : IRequest<Tuple<IEnumerable<GetWard>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Query
{
    public class GetAllGSTQuery : IRequest<Tuple<IEnumerable<GetGST>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
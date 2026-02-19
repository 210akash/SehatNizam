using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Query
{
    public class GetAllStoreQuery : IRequest<Tuple<IEnumerable<GetStore>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
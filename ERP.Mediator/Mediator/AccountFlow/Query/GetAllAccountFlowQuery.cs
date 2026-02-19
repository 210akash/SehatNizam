using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AccountFlow.Query
{
    public class GetAllAccountFlowQuery : IRequest<Tuple<IEnumerable<GetAccountFlow>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
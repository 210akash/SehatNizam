using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Query
{
    public class GetAllAccountHeadQuery : IRequest<Tuple<IEnumerable<GetAccountHead>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
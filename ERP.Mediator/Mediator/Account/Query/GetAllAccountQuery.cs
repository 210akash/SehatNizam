using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class GetAllAccountQuery : IRequest<Tuple<IEnumerable<GetAccount>, long>>
    {
        public long? AccountTypeId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
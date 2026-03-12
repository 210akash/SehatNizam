using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Query
{
    public class GetAllAccountCategoryQuery : IRequest<Tuple<IEnumerable<GetAccountCategory>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
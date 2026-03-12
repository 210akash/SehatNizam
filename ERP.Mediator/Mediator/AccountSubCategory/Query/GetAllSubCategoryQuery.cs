using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAllAccountSubCategoryQuery : IRequest<Tuple<IEnumerable<GetAccountSubCategory>, long>>
    {
        public long? AccountCategoryId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
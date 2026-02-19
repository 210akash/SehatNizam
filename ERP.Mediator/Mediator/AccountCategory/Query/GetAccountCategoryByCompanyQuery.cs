using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Query
{
    public class GetAccountCategoryByCompanyQuery : IRequest<List<GetAccountCategory>>
    {
        public GetAccountCategoryByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}
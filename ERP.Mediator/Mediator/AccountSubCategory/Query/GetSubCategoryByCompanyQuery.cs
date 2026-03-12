using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAccountSubCategoryByCompanyQuery : IRequest<List<GetAccountSubCategory>>
    {
        public GetAccountSubCategoryByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}
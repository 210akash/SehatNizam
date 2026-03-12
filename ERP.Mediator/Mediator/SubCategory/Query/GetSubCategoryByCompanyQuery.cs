using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetSubCategoryByCompanyQuery : IRequest<List<GetSubCategory>>
    {
        public GetSubCategoryByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}
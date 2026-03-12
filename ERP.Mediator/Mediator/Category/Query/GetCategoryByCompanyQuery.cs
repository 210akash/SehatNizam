using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Query
{
    public class GetCategoryByCompanyQuery : IRequest<List<GetCategory>>
    {
        public GetCategoryByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}
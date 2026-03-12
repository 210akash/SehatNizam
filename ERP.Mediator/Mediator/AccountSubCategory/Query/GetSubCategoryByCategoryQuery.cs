using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAccountSubCategoryByCategoryQuery : IRequest<List<GetAccountSubCategory>>
    {
        public GetAccountSubCategoryByCategoryQuery(long AccountCategoryId)
        {
            this.AccountCategoryId = AccountCategoryId;
        }

        public long AccountCategoryId { get; set; }
    }
}
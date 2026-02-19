using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAccountTypeByAccountSubCategoryQuery : IRequest<List<GetAccountType>>
    {
        public GetAccountTypeByAccountSubCategoryQuery(long AccountSubCategoryId)
        {
            this.AccountSubCategoryId = AccountSubCategoryId;
        }

        public long AccountSubCategoryId { get; set; }
    }
}
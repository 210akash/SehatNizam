using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAccountSubCategoryByNameQuery : IRequest<List<GetAccountSubCategory>>
    {
        public GetAccountSubCategoryByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
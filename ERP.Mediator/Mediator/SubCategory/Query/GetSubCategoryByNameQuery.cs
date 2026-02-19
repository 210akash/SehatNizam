using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetSubCategoryByNameQuery : IRequest<List<GetSubCategory>>
    {
        public GetSubCategoryByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetSubCategoryByCategoryQuery : IRequest<List<GetSubCategory>>
    {
        public GetSubCategoryByCategoryQuery(long CategoryId)
        {
            this.CategoryId = CategoryId;
        }

        public long CategoryId { get; set; }
    }
}
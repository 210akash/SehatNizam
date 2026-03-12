using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetItemtypeBySubCategoryQuery : IRequest<List<GetItemType>>
    {
        public GetItemtypeBySubCategoryQuery(long SubCategoryId)
        {
            this.SubCategoryId = SubCategoryId;
        }

        public long SubCategoryId { get; set; }
    }
}
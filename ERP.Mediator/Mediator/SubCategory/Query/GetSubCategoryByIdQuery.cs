using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetSubCategoryByIdQuery : IRequest<GetSubCategory>
    {
        public GetSubCategoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
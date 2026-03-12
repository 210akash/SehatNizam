using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAccountSubCategoryByIdQuery : IRequest<GetAccountSubCategory>
    {
        public GetAccountSubCategoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
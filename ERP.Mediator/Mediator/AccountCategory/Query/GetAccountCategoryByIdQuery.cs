using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Query
{
    public class GetAccountCategoryByIdQuery : IRequest<GetAccountCategory>
    {
        public GetAccountCategoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
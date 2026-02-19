using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class GetAccountSubCategoryCodeQuery : IRequest<string>
    {
        public GetAccountSubCategoryCodeQuery(long AccountCategoryId, long Id)
        {
            this.AccountCategoryId = AccountCategoryId;
            this.Id = Id;
        }
        public long AccountCategoryId { get; set; }
        public long Id { get; set; }
    }

}
using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Query
{
    public class GetAccountTypeCodeQuery : IRequest<string>
    {
        public GetAccountTypeCodeQuery(long AccountSubCategoryId,long Id)
        {
            this.AccountSubCategoryId = AccountSubCategoryId;
            this.Id = Id;
        }
        public long AccountSubCategoryId { get; set; }
        public long Id { get; set; }
    }

}
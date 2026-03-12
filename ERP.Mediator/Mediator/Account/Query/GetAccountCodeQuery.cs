using MediatR;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class GetAccountCodeQuery : IRequest<string>
    {
        public GetAccountCodeQuery(long AccountTypeId,long Id)
        {
            this.AccountTypeId = AccountTypeId;
            this.Id = Id;
        }
        public long AccountTypeId { get; set; }
        public long Id { get; set; }
    }

}
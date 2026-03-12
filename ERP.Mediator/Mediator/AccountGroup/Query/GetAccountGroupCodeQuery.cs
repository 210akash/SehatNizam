using MediatR;

namespace ERP.Mediator.Mediator.AccountGroup.Query
{
    public class GetAccountGroupCodeQuery : IRequest<string>
    {
        public GetAccountGroupCodeQuery(long AccountId, long Id)
        {
            this.AccountId = AccountId;
            this.Id = Id;
        }
        public long AccountId { get; set; }
        public long Id { get; set; }
    }

}
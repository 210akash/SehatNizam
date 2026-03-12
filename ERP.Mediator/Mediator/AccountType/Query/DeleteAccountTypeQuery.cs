using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Query
{
    public class DeleteAccountTypeQuery : IRequest<bool>
    {
        public DeleteAccountTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
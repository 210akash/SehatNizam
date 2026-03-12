using MediatR;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class DeleteAccountQuery : IRequest<bool>
    {
        public DeleteAccountQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
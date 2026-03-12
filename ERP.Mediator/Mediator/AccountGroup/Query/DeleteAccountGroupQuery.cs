using MediatR;

namespace ERP.Mediator.Mediator.AccountGroup.Query
{
    public class DeleteAccountGroupQuery : IRequest<bool>
    {
        public DeleteAccountGroupQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
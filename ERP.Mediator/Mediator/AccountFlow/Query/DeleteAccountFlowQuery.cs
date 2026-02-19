using MediatR;

namespace ERP.Mediator.Mediator.AccountFlow.Query
{
    public class DeleteAccountFlowQuery : IRequest<bool>
    {
        public DeleteAccountFlowQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Query
{
    public class DeleteAccountHeadQuery : IRequest<bool>
    {
        public DeleteAccountHeadQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
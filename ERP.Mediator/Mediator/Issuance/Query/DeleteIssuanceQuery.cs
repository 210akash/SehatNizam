using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class DeleteIssuanceQuery : IRequest<bool>
    {
        public DeleteIssuanceQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
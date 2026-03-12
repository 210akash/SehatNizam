using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class ProcessIssuanceQuery : IRequest<bool>
    {
        public ProcessIssuanceQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
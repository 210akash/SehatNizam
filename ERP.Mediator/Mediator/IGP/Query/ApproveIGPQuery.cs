using MediatR;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class ApproveIGPQuery : IRequest<bool>
    {
        public ApproveIGPQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
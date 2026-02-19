using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Query
{
    public class ApproveIndentRequestQuery : IRequest<bool>
    {
        public ApproveIndentRequestQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
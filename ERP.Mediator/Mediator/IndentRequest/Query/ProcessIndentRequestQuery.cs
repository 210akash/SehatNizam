using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Query
{
    public class ProcessIndentRequestQuery : IRequest<bool>
    {
        public ProcessIndentRequestQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
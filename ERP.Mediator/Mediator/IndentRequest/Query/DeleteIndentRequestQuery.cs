using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Query
{
    public class DeleteIndentRequestQuery : IRequest<bool>
    {
        public DeleteIndentRequestQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
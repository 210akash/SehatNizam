using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Query
{
    public class DeleteIndentTypeQuery : IRequest<bool>
    {
        public DeleteIndentTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
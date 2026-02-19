using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class DeleteComparativeStatementQuery : IRequest<bool>
    {
        public DeleteComparativeStatementQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
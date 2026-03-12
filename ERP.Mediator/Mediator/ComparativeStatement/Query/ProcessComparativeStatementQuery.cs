using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class ProcessComparativeStatementQuery : IRequest<bool>
    {
        public ProcessComparativeStatementQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
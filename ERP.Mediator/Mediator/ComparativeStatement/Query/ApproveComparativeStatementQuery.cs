using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class ApproveComparativeStatementQuery : IRequest<bool>
    {
        public ApproveComparativeStatementQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
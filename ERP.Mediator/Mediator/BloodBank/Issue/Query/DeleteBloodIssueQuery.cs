using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Query
{
    public class DeleteBloodIssueQuery : IRequest<long>
    {
        public long Id { get; set; }
        public DeleteBloodIssueQuery(long id) { Id = id; }
    }
}

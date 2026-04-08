using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class ApproveRosterQuery : IRequest<bool>
    {
        public ApproveRosterQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
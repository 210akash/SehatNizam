using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class RejectRosterQuery : IRequest<bool>
    {
        public RejectRosterQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
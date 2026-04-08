using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class DeleteRosterQuery : IRequest<bool>
    {
        public DeleteRosterQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
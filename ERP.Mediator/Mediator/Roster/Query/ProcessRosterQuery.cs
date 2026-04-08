using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class ProcessRosterQuery : IRequest<bool>
    {
        public ProcessRosterQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
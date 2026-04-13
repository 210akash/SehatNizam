using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Roster.Command
{
    public class SaveRosterByManagerCommand : IRequest<long>
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Remarks { get; set; }
        public virtual List<SaveRosterDetailCommand> RosterDetail { get; set; }
    }
}

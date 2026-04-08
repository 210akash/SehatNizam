using MediatR;
using System.Collections.Generic;
using System;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Roster.Command
{
    public class SaveRosterCommand : IRequest<long>
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long DepartmentId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public virtual List<SaveRosterDetailCommand> RosterDetail { get; set; }
    }

    public class SaveRosterDetailCommand
    {
        public long Id { get; set; }
        public long RosterId { get; set; }
        public Guid EmployeeId { get; set; }
        public long EmployeeShiftId { get; set; }
        public DateTime RosterDate { get; set; }
        public bool IsOffDay { get; set; } = false;
    }
}

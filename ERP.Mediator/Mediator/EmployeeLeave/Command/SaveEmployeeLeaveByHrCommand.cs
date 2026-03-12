using ERP.Entities.Models;
using MediatR;
using System;

namespace ERP.Mediator.Mediator.EmployeeLeave.Command
{
    public class SaveEmployeeLeaveByHrCommand : IRequest<string>
    {
        public Guid EmployeeId { get; set; }
        public long EmployeeGroupLeaveTypeDetailId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsFirstHalfDay { get; set; } = false;
        public bool? IsLastHalfDay { get; set; } = false;
        public string Reason { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Command
{
    public class SaveEmployeeGroupLeaveTypeCommand : IRequest<long>
    {
        public long EmployeeLeaveGroupId { get; set; }
        public List<EmployeeGroupLeaveTypeCommand> EmployeeGroupLeaveType { get; set; }
    }

    public class EmployeeGroupLeaveTypeCommand
    {
        public long Id { get; set; }
        public long EmployeeLeaveGroupId { get; set; }
        public long HRYearId { get; set; }
        public List<EmployeeGroupLeaveTypeDetailCommand> EmployeeGroupLeaveTypeDetail { get; set; }
    }

    public class EmployeeGroupLeaveTypeDetailCommand
    {
        public long Id { get; set; }
        public long? NoOfLeaves { get; set; }
        public long EmployeeLeaveTypeId { get; set; }
        public long EmployeeGroupLeaveTypeId { get; set; }

    }
}

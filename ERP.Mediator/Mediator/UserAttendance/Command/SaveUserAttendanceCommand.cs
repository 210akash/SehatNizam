using MediatR;
using System;

namespace ERP.Mediator.Mediator.UserAttendance.Command
{
    public class SaveUserAttendanceCommand : IRequest<long>
    {
        public long Id { get; set; }
        public Guid? UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public int AttendanceType { get; set; }
        public int DeviceType { get; set; }
        public long? EmployeeShiftId { get; set; }
        public bool IsManualIn { get; set; } = false;
        public bool IsManualOut { get; set; } = false;
    }
}

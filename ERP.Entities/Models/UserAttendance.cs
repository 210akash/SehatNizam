using System;

namespace ERP.Entities.Models
{
    public class UserAttendance : BaseEntity
    {
        public Guid? UserId { get; set; }
        public virtual AspNetUsers User { get; set; }
        public bool? IsPresent { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Reason { get; set; }
        public string PinLocation { get; set; }
        public string CheckOutLocation { get; set; }
        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public decimal? WorkingHours { get; set; }

        public decimal? OverTimeHours { get; set; }

        public int? AttendanceType { get; set; }

        public int? DeviceType { get; set; }

        public decimal? Attendance { get; set; }

        public string Description { get; set; }

        public long? EmployeeShiftId { get; set; }
        public EmployeeShift EmployeeShift { get; set; }

        public long? InDeviceId { get; set; }
        public Device InDevice { get; set; }

        public long? OutDeviceId { get; set; }
        public Device OutDevice { get; set; }
        public bool IsManualIn { get; set; } = false;
        public bool IsManualOut { get; set; } = false;

        public Guid? ManualById { get; set; }
        public virtual AspNetUsers ManualBy { get; set; }
    }
}

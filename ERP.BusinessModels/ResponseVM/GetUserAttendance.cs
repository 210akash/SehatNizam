using ERP.Entities.Models;
using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetUserAttendance
    {
        public long Id { get; set; }
        public DateTime? CreatedDate { get; set; }

        public Guid? UserId { get; set; }
        public GetUsers User { get; set; }
        public bool? IsPresent { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Reason { get; set; }
        public string PinLocation { get; set; }
        public string TerritoryName { get; set; }
        public string ZoneName { get; set; }
        public string DealershipName { get; set; }
        public DateTime? CheckOut { get; set; }
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
        public GetEmployeeShift EmployeeShift { get; set; }

        public long? InDeviceId { get; set; }
        public GetDevice InDevice { get; set; }

        public long? OutDeviceId { get; set; }
        public GetDevice OutDevice { get; set; }
        public string Status { get; set; }

        public bool IsManualIn { get; set; } = false;
        public bool IsManualOut { get; set; } = false;
        public GetUsers ManualBy { get; set; }
    }
}

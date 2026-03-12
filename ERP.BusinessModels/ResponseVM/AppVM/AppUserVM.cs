using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM.AppVM
{
    public class AppUserVM
    {
        public Guid UserId { get; set; }
        public bool IsLoginSuccess { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ProfileBlobURl { get; set; }
        public string PhoneNumber { get; set; }
        public TimeSpan? ShiftTimeStart { get; set; }
        public TimeSpan? ShiftTimeEnd { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string Image { get; set; }
        public bool IsMarkAttendance { get; set; } = false;
        public bool? IsPresent { get; set; }
        public DateTime? PresentTime { get; set; }
        public bool? IsCheckOut { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string AbsentReason { get; set; }
        public bool? IsNewDevice { get; set; }

        public long? DealershipId { get; set; }
        public string? DealershipName { get; set; }
        public string? DealershipLocation { get; set; }
        public double? DistanceInMeters { get; set; } = 0;
        public string FormattedDistance { get; set; }
        public string FormattedDistanceUnit { get; set; }
        public string DeviceId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public bool? IsLogedIn { get; set; }
        public string EmployeeDesignation { get; set; }
        public List<DealershipDetails> lstDealershipDetails { get; set; }
    }
    public class DealershipDetails
    {
        public long? DealershipId { get; set; }
        public string DealershipName { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string DealershipLocation { get; set; }
        public double? DistanceInMeters { get; set; } = 0;
        public string FormattedDistance { get; set; }
        public string FormattedDistanceUnit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetUsers
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string EmailConfirmationCode { get; set; }
        public DateTime? EmailCodeExpiryDateTime { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public long CompanyId { get; set; }
        public string Title { get; set; }
        public string TimeZone { get; set; }
        public string ProfileBlobUrl { get; set; }
        public bool? IsDeviceWizardComplete { get; set; }
        public string UID { get; set; }
        public int? SecurityMehtod2FA { get; set; }
        public string Code { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyPhoneNo { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public TimeSpan? ShiftTimeStart { get; set; }
        public TimeSpan? ShiftTimeEnd { get; set; }
        public Guid CreatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public long? DealershipId { get; set; }
        public string RoleId { get; set; }
        public List<AspNetUserRoles> AspNetUserRoles { get; set; }
        public List<GetDSFRoute> DSFRoute { get; set; }
        public List<GetAttachments> Attachments { get; set; }
        public List<GetUserTerritory> UserTerritory { get; set; }
        public List<GetOrder> Orders { get; set; }
        public string DeviceId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public string HrGuid { get; set; }
        public string WeeklyOff { get; set; }

        public bool IsEmployee { get; set; }

        public List<GetUserProject> UserProject { get; set; }

    }
}

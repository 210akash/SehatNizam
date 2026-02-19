using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
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
        public bool IsActive { get; set; }
        public bool? IsDeviceWizardComplete { get; set; }
        public string UID { get; set; }
        public int? SecurityMehtod2FA { get; set; }
        public string Code { get; set; }
        public GetDepartment Department { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ERP.Entities.Models
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
        }

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
        public long? DepartmentId { get; set; }

        #region HR
        public bool IsEmployee { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime? CNICIssuanceDate { get; set; }
        public DateTime? CNICExpiryDate { get; set; }
        public string Religion { get; set; }
        public string FatherHusbandName { get; set; }
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public string SpouseCNIC { get; set; }
        public string Child1 { get; set; }
        public string Child2 { get; set; }
        public string SubDepartment { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Country { get; set; }
        public string PermanentAddress { get; set; }
        public string EmergencyPersonName { get; set; }
        public string EmergencyRelation { get; set; }
        public string AccountHolderName { get; set; }
        public string BankAccountIBAN { get; set; }
        public string BankAccountNo { get; set; }
        public string OverTimeAmount { get; set; }
        public DateTime? DateOfConfirmation { get; set; }

        public long? EmployeeDesignationId { get; set; }
        public virtual EmployeeDesignation EmployeeDesignation { get; set; }

        public long? EmployeeEducationId { get; set; }
        public virtual EmployeeEducation EmployeeEducation { get; set; }

        public long? EmployeeGradeId { get; set; }
        public virtual EmployeeGrade EmployeeGrade { get; set; }

        public long? EmployeeShiftId { get; set; }
        public virtual EmployeeShift EmployeeShift { get; set; }

        public long? EmployeeTypeId { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }

        public long? EmployeeLeaveGroupId { get; set; }
        public virtual EmployeeLeaveGroup EmployeeLeaveGroup { get; set; }

        public long? EmployeeBankId { get; set; }
        public virtual EmployeeBank EmployeeBank { get; set; }

        public long? EmployeeWorkSiteTypeId { get; set; }
        public virtual EmployeeWorkSiteType EmployeeWorkSiteType { get; set; }
        public string WorkLocation { get; set; }

        [MaxLength(100)]
        public string WorkCity { get; set; }

        //public long? StatusId { get; set; }
        //public virtual Status Status { get; set; }

        public long? CityId { get; set; }
        public virtual City City { get; set; }
        public DateTime? ResignDate { get; set; }
        public bool IsResigned { get; set; } = false;
        public long? EmployeeOvertimeRateId { get; set; }
        public virtual EmployeeOvertimeRate EmployeeOvertimeRate { get; set; }
        public string LastCompany { get; set; }
        public string RelevantExperience { get; set; }
        public string TotalWorkExperience { get; set; }
        public string Reference { get; set; }
        public string Remarks { get; set; }
        public virtual ICollection<EmployeeWorkingDays> EmployeeWorkingDays { get; set; }
        public virtual ICollection<RosterDetail> RosterDetail { get; set; }
        public virtual ICollection<EmployeeDocument> EmployeeDocument { get; set; }

        #endregion

        [JsonIgnore] // Prevent cycles
        public virtual Department Department { get; set; }

        public long? StoreId { get; set; }

        [JsonIgnore] // Prevent cycles
        public virtual Store Store { get; set; }

        public string Title { get; set; }
        public string TimeZone { get; set; }
        public string ProfileBlobUrl { get; set; }
        public bool? IsDeviceWizardComplete { get; set; }
        public string UID { get; set; }
        public int? SecurityMehtod2FA { get; set; }

        [Display(Name = "MRN Number")]
        public string Code { get; set; }
        public string HrCode { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyPhoneNo { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public TimeSpan? ShiftTimeStart { get; set; }
        public TimeSpan? ShiftTimeEnd { get; set; }
        public string DeviceId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public Guid CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsLogedIn { get; set; }
        public bool IsRosterShift { get; set; }
        public string WeeklyOff { get; set; }

        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }

        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<UserTerritory> UserTerritory { get; set; }
        public virtual ICollection<DSFRoute> DSFRoute { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        //User Warehouse
        public virtual ICollection<UserProject> UserProject { get; set; }

        public virtual ICollection<Attachments> Attachments { get; set; }
        public virtual ICollection<UserAttendance> UserAttendance { get; set; }
        public virtual ICollection<EmployeeDevice> EmployeeDevice { get; set; }

        #region Appoinment 
        public ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
        #endregion
    }
}

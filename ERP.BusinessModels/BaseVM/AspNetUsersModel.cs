//-----------------------------------------------------------------------
// <copyright file="AspNetUsersModel.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.BaseVM
{
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Declaration of Asp Net Users Model class.
    /// </summary>
    public class AspNetUsersModel : BaseEntityModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the normalized user.
        /// </summary>
        /// <value>
        /// The name of the normalized user.
        /// </value>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the normalized email.
        /// </summary>
        /// <value>
        /// The normalized email.
        /// </value>
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [email confirmed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [email confirmed]; otherwise, <c>false</c>.
        /// </value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>
        /// The password hash.
        /// </value>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the security stamp.
        /// </summary>
        /// <value>
        /// The security stamp.
        /// </value>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Gets or sets the concurrency stamp.
        /// </summary>
        /// <value>
        /// The concurrency stamp.
        /// </value>
        public string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has Phone Number Confirmed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has Phone Number Confirmed; otherwise, <c>false</c>.
        /// </value>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has Two Factor Enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has Two Factor Enabled; otherwise, <c>false</c>.
        /// </value>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the lockout end.
        /// </summary>
        /// <value>
        /// The lockout end.
        /// </value>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has Lockout Enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has Two Lockout Enabled; otherwise, <c>false</c>.
        /// </value>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        /// <value>
        /// The user role id.
        /// </value>
        public Guid[]? RoleId { get;set; }
        public string RoleName { get;set; }

        public bool IsRemember { get; set; }

        /// <summary>
        /// Gets or sets the access failed count.
        /// </summary>
        /// <value>
        /// The access failed count.
        /// </value>
        public int AccessFailedCount { get; set; }
        public string TimeZone { get; set; }
        public string Title { get; set; }
        public string ProfileBlobUrl { get; set; }
        public bool IsActive { get; set; }
        public bool HaveAssetAccess { get; set; }
        public int? SecurityMehtod2FA { get; set; }
        public bool? IsDeviceWizardComplete { get; set; }
        public string Code { get; set; }

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
        //public string BusinessUnit { get; set; }
        //public string WorkLevel { get; set; }
        //public string LineManagerName { get; set; }
        //public string LineManagerDesignation { get; set; }
        public string Country { get; set; }
        public string PermanentAddress { get; set; }
        public string EmergencyPersonName { get; set; }
        public string EmergencyRelation { get; set; }
        //public string BankName { get; set; }
        //public string BranchName { get; set; }
        public string AccountHolderName { get; set; }
        //public string AccountNumber { get; set; }
        //public string BranchCode { get; set; }
        public string BankAccountIBAN { get; set; }
        public string BankAccountNo { get; set; }
        //public string OtherLeaves { get; set; }
        //public string EmergencyLeave { get; set; }
        //public string SickLeave { get; set; }
        //public string CasualLeave { get; set; }
        public string OverTimeAmount { get; set; }
        public DateTime? DateOfConfirmation { get; set; }
        //public string ServiceStatusDescription { get; set; }
        public long? EmployeeDesignationId { get; set; }
        public long? EmployeeEducationId { get; set; }
        public long? EmployeeGradeId { get; set; }
        public long? EmployeeShiftId { get; set; }
        public long? EmployeeTypeId { get; set; }
        public long? EmployeeWorkSiteTypeId { get; set; }
        public long? EmployeeLeaveGroupId { get; set; }
        public long? EmployeeBankId { get; set; }
        public long? CityId { get; set; }
        public string HrCode { get; set; }
        public DateTime? ResignDate { get; set; }
        public bool IsResigned { get; set; } = false;
        public long? EmployeeOvertimeRateId { get; set; }
        public string LastCompany { get; set; }
        public string RelevantExperience { get; set; }
        public string TotalWorkExperience { get; set; }
        public string Reference { get; set; }
        public string Remarks { get; set; }
        public GetEmployeeWorkingDays Days { get; set; }
        public List<ImageUploadModel> Documents { get; set; }

        #endregion

        public long? DepartmentId { get; set; }
        public Department Department { get; set; }

        public long? StoreId { get; set; }
        public Store Store { get; set; }


        // Fields for KC Users (SALE) START
        public string DeviceId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public string WeeklyOff { get; set; }
        public bool? IsLogedIn { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyPhoneNo { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public TimeSpan? ShiftTimeStart { get; set; }
        public TimeSpan? ShiftTimeEnd { get; set; }
        public Guid CreatedById { get; set; }
        public bool IsDelete { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public long? DealershipId { get; set; }
        public GetDealership Dealership { get; set; }
        // Fields for KC Users (SALE) END

        //User Warehouse
        public long? SelectedWarehouseId { get; set; }
        public long? RetailUserShopId { get; set; }
        public List<UserProject> UserProject { get; set; }

        public List<long> ProjectIds { get; set; }
    }
}
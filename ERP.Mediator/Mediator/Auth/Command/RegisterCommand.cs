//-----------------------------------------------------------------------
// <copyright file="RegisterCommand.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using System.Collections.Generic;
using ERP.BusinessModels.ParameterVM;

namespace ERP.Mediator.Mediator.Auth.Command
{
    /// <summary>
    /// Declaration of Register Model class.
    /// </summary>
    public class RegisterCommand : IRequest<IdentityResponse>
    {
        /// <summary>
        /// Gets or sets of user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets of first name
        /// </summary>
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(500, ErrorMessage = "First Name must be between 3 and 500 characters", MinimumLength = 3)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets of last name
        /// </summary>
        //[Required(ErrorMessage = "Last Name is required")]
        //[StringLength(500, ErrorMessage = "Last Name must be between 3 and 500 characters", MinimumLength = 3)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets of email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets of password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        /// <value>
        /// The company identifier.
        /// </value>
        public string CompanyName { get; set; }
        public long? DepartmentId { get; set; }
        public long? StoreId { get; set; }
        public int? IndustryId { get; set; }
        public string Title { get; set; }
        public string[]? RoleId { get; set; }
        public int Age { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyPhoneNo { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ShiftTimeStart { get; set; }
        public string ShiftTimeEnd { get; set; }
        public string ImageName { get; set; }
        public string FileSource { get; set; }
        public string Extension { get; set; }
        public long? DealershipId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public string WeeklyOff { get; set; }

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
        public string WorkLocation { get; set; }
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
        public List<long> ProjectIds { get; set; }

        #endregion
    }
}
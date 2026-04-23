using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAllUsers
    {
        [Key]
        public long RowNumber { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public string[]? RoleId { get; set; }
        public string ProfileBlobUrl { get; set; }
        public string Code { get; set; }
        public string HrCode { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? InvitationDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool? IsCancel { get; set; }
        public long? CompanyId { get; set; }

        public long? StoreId { get; set; }
        public GetStore Store { get; set; }


        public long? DepartmentId { get; set; }
        public GetDepartment Department { get; set; }

        public List<AspNetUserRoles> AspNetUserRoles { get; set; }


        // Fields for KC Users (SALE) START
        public string DeviceId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public string WeeklyOff { get; set; }
        public long? DealershipId { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyPhoneNo { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public TimeSpan? ShiftTimeStart { get; set; }
        public TimeSpan? ShiftTimeEnd { get; set; }
        public bool IsRosterShift { get; set; }
        public List<GetDSFRoute> DSFRoute { get; set; }
        public List<GetAttachments> Attachments { get; set; }
        public List<GetUserTerritory> UserTerritory { get; set; }
        public List<GetOrder> Orders { get; set; }
        // Fields for KC Users (SALE) END

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
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? EmployeeDesignationId { get; set; }
        public GetEmployeeDesignation EmployeeDesignation { get; set; }
        public long? EmployeeEducationId { get; set; }
        public GetEmployeeEducation EmployeeEducation { get; set; }
        public long? EmployeeGradeId { get; set; }
        public GetEmployeeGrade EmployeeGrade { get; set; }
        public long? EmployeeShiftId { get; set; }
        public GetEmployeeShift EmployeeShift { get; set; }
        public long? EmployeeTypeId { get; set; }
        public GetEmployeeType EmployeeType { get; set; }

        public long? EmployeeWorkSiteTypeId { get; set; }
        public GetEmployeeWorkSiteType EmployeeWorkSiteType { get; set; }
        public string WorkLocation { get; set; }
        public long? EmployeeLeaveGroupId { get; set; }
        public GetEmployeeLeaveGroup EmployeeLeaveGroup { get; set; }

        public long? EmployeeBankId { get; set; }
        public GetEmployeeBank EmployeeBank { get; set; }
        public long? CityId { get; set; }
        public GetCity City { get; set; }

        public DateTime? ResignDate { get; set; }
        public bool IsResigned { get; set; } = false;
        public long? EmployeeOvertimeRateId { get; set; }
        public virtual GetEmployeeOvertimeRate EmployeeOvertimeRate { get; set; }
        public string LastCompany { get; set; }
        public string RelevantExperience { get; set; }
        public string TotalWorkExperience { get; set; }
        public string Reference { get; set; }
        public string Remarks { get; set; }

        public List<GetEmployeeWorkingDays> EmployeeWorkingDays { get; set; }
        public List<GetEmployeeDocument> EmployeeDocument { get; set; }

        //User Warehouse
        public List<long> ProjectIds { get; set; }
        public List<GetUserProject> UserProject { get; set; }

        // Employee Device
        public List<GetEmployeeDevice> EmployeeDevice { get; set; }
        public List<GetEmployeeSalary> EmployeeSalary { get; set; }

        #endregion
    }
}

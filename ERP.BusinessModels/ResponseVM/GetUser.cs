using System;
using System.Collections.Generic;
using ERP.Entities.Models;

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

        #region HR

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
        public long? EmployeeLeaveGroupId { get; set; }
        public long? EmployeeBankId { get; set; }
        public long? CityId { get; set; }
        public List<GetEmployeeWorkingDays> EmployeeWorkingDays { get; set; }
        public List<GetEmployeeDocument> EmployeeDocument { get; set; }

        #endregion
    }
}

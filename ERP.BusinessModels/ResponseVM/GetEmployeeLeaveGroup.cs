using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeLeaveGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long EmployeeLeaveTypeId { get; set; }
        public GetUser CreatedBy { get; set; }

        public List<GetEmployeeGroupLeaveType> EmployeeGroupLeaveType { get; set; }
    }

    public class GetEmployeeGroupLeaveType
    {
        public long Id { get; set; }
        public long EmployeeLeaveGroupId { get; set; }
        public long HRYearId { get; set; }
        public GetHRYear HRYear { get; set; }
        public List<GetEmployeeGroupLeaveTypeDetail> EmployeeGroupLeaveTypeDetail { get; set; }
    }

    public class GetEmployeeGroupLeaveTypeDetail
    {
        public long Id { get; set; }
        public long EmployeeGroupLeaveTypeId { get; set; }
        public GetEmployeeGroupLeaveType EmployeeGroupLeaveType { get; set; }
        public long NoOfLeaves { get; set; }
        public long EmployeeLeaveTypeId { get; set; }
        public GetEmployeeLeaveType EmployeeLeaveType { get; set; }
    }

    public class LeaveBalanceDto
    {
        public long Id { get; set; }         // e.g., "Casual Leave"
        public string LeaveType { get; set; }         // e.g., "Casual Leave"
        public decimal Allotted { get; set; }         // Total leaves given
        public double Used { get; set; }              // Total used (including half-days)
        public double Balance { get; set; }           // Remaining leave
    }


}
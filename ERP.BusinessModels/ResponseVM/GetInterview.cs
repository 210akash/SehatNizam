using System.Collections.Generic;
using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetInterview
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public decimal Experience { get; set; }
        public string Reference { get; set; }
        public string PersonalDetail { get; set; }
        public string Reason { get; set; }
        public DateTime? JoinDate { get; set; }

        public GetUser CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long EmployeeEducationId { get; set; }
        public GetEmployeeEducation EmployeeEducation { get; set; }

        public long DepartmentId { get; set; }
        public GetDepartment Department { get; set; }

        public long EmployeeDesignationId { get; set; }
        public GetEmployeeDesignation EmployeeDesignation { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }

        public List<GetInterviewHistory> InterviewHistory { get; set; }
        public List<GetAttachments> Attachments { get; set; }
    }

    public class GetInterviewHistory
    {
        public long Id { get; set; }
        public DateTime? InterviewDate { get; set; }
        public int? JoinAfterDays { get; set; }
        public string Comments { get; set; }

        public GetUser CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public long InterviewId { get; set; }
        public GetInterview Interview { get; set; }

        public List<GetInterviewAttendees> InterviewAttendees { get; set; }
    }

    public class GetInterviewAttendees
    {
        public long InterviewHistoryId { get; set; }
        public GetInterviewHistory InterviewHistory { get; set; }

        public Guid AspNetUsersId { get; set; }
        public GetUser AspNetUsers { get; set; }
    }
}

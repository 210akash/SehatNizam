using ERP.Entities.Models;
using ERP.Mediator.Mediator.Company.Command;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Interview.Command
{
    public class SaveInterviewCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public long EmployeeEducationId { get; set; }
        public long DepartmentId { get; set; }
        public long EmployeeDesignationId { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public decimal Experience { get; set; }
        public string Reference { get; set; }
        public string PersonalDetail { get; set; }
        public string Reason { get; set; }
        public DateTime? JoinDate { get; set; }
        public long CompanyId { get; set; }
        public List<FileCommand> FileCommand { get; set; }

        //public DateTime? InterviewDate { get; set; }
        //public int? JoinAfterDays { get; set; }
        public string Remarks { get; set; }

        //public string[] InterviewAttendees { get; set; }
    }
}

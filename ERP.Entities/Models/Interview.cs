using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Interview : BaseEntity
    {
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Mobile { get; set; }

        [MaxLength(25)]
        public string Email { get; set; }
        public long EmployeeEducationId { get; set; }
        public virtual EmployeeEducation EmployeeEducation { get; set; }
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public long EmployeeDesignationId { get; set; }
        public virtual EmployeeDesignation EmployeeDesignation { get; set; }
        public long StatusId { get; set; }
        public virtual Status Status { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public decimal Experience { get; set; }
        public string Reference { get; set; }

        [MaxLength(500)]
        public string PersonalDetail { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }

        [MaxLength(500)]
        public string LastCompany { get; set; }

        [MaxLength(1000)]
        public string Remarks { get; set; }
        public DateTime? JoinDate { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual List<InterviewHistory> InterviewHistory { get; set; }
        public virtual List<Attachments> Attachments { get; set; }
        public virtual List<CandidateEvaluation> CandidateEvaluation { get; set; }
    }
}

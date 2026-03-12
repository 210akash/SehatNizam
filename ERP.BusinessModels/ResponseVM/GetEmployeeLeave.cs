using ERP.Entities.Models;
using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeLeave
    {
        public long Id { get; set; }
        public Guid EmployeeId { get; set; }
        public virtual GetAllUsers Employee { get; set; }
        public long EmployeeGroupLeaveTypeDetailId { get; set; }
        public virtual GetEmployeeGroupLeaveTypeDetail EmployeeGroupLeaveTypeDetail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsFirstHalfDay { get; set; }
        public bool IsLastHalfDay { get; set; }
        public string Reason { get; set; }
        public string Comments { get; set; }
        public long StatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public GetStatus Status { get; set; }
        public GetUser CreatedBy { get; set; }
        public GetUser ModifiedBy { get; set; }
        public GetUser ProcessedBy { get; set; }
        public GetUser ApprovedBy { get; set; }
    }
}

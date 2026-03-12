using System;
namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeDevice
    {
        public long Id { get; set; }
        public Guid EmployeeId { get; set; }
        public  GetUser Employee { get; set; }
        public long DeviceId { get; set; }
        public virtual GetDevice Device { get; set; }
        public string EnrollmentNo { get; set; }
        public bool IsSyned { get; set; }
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}

using System;

namespace ERP.Entities.Models
{
    public class EmployeeDevice : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }
        public long DeviceId { get; set; }
        public virtual Device Device { get; set; }
        public string EnrollmentNo { get; set; }
        public bool IsSyned { get; set; } = false;
    }
}

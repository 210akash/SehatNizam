using System;

namespace ERP.Entities.Models
{
    public class BloodRequest : BaseEntity
    {
        public string Code { get; set; }
        public long? AdmissionId { get; set; }
        public virtual Admission Admission { get; set; }
        public string PatientName { get; set; }
        public string PatientCNIC { get; set; }
        public long BloodGroupMasterId { get; set; }
        public virtual BloodGroupMaster BloodGroupMaster { get; set; }
        public long BloodComponentTypeId { get; set; }
        public virtual BloodComponentType BloodComponentType { get; set; }
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
    }
}

using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodRequest
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long? AdmissionId { get; set; }
        public GetAdmission Admission { get; set; }
        public string PatientName { get; set; }
        public string PatientCNIC { get; set; }
        public long BloodGroupMasterId { get; set; }
        public GetBloodGroupMaster BloodGroupMaster { get; set; }
        public long BloodComponentTypeId { get; set; }
        public GetBloodComponentType BloodComponentType { get; set; }
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}

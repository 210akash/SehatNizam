using System;

namespace ERP.Entities.Models
{
    public class BloodDonor : BaseEntity
    {
        public string DonorCode { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long? BloodGroupMasterId { get; set; }
        public virtual BloodGroupMaster BloodGroupMaster { get; set; }
        public long? PatientMasterId { get; set; }
        public virtual PatientMaster PatientMaster { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public bool IsDeferred { get; set; }
        public string DeferralReason { get; set; }
    }
}

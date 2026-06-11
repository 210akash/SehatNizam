using System;

namespace ERP.Entities.Models
{
    public class BloodDonation : BaseEntity
    {
        public string DonationCode { get; set; }
        public long BloodDonorId { get; set; }
        public virtual BloodDonor BloodDonor { get; set; }
        public long BloodComponentTypeId { get; set; }
        public virtual BloodComponentType BloodComponentType { get; set; }
        public long? BloodGroupMasterId { get; set; }
        public virtual BloodGroupMaster BloodGroupMaster { get; set; }
        public DateTime DonationDate { get; set; }
        public decimal Volume { get; set; }
        public int ScreeningStatus { get; set; }
        public string Remarks { get; set; }
    }
}

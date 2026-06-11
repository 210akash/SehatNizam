using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodDonation
    {
        public long Id { get; set; }
        public string DonationCode { get; set; }
        public long BloodDonorId { get; set; }
        public GetBloodDonor BloodDonor { get; set; }
        public long BloodComponentTypeId { get; set; }
        public GetBloodComponentType BloodComponentType { get; set; }
        public long? BloodGroupMasterId { get; set; }
        public GetBloodGroupMaster BloodGroupMaster { get; set; }
        public DateTime DonationDate { get; set; }
        public decimal Volume { get; set; }
        public int ScreeningStatus { get; set; }
        public string Remarks { get; set; }
        public GetBloodUnit BloodUnit { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}

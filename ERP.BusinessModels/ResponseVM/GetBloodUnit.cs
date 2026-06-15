using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodUnit
    {
        public long Id { get; set; }
        public string UnitNo { get; set; }
        public string Barcode { get; set; }
        public long? BloodDonationId { get; set; }
        public long? DonationAppointmentId { get; set; }
        public long BloodComponentTypeId { get; set; }
        public GetBloodComponentType BloodComponentType { get; set; }
        public long BloodGroupMasterId { get; set; }
        public GetBloodGroupMaster BloodGroupMaster { get; set; }
        public decimal Volume { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public long? BloodFridgeId { get; set; }
        public GetBloodFridge BloodFridge { get; set; }
        public long? BloodRackId { get; set; }
        public GetBloodRack BloodRack { get; set; }
        public string SlotNo { get; set; }
        public int Status { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}

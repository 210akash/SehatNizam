using System;

namespace ERP.Entities.Models
{
    public class BloodUnit : BaseEntity
    {
        public string UnitNo { get; set; }
        public string Barcode { get; set; }
        public long? BloodDonationId { get; set; }
        public virtual BloodDonation BloodDonation { get; set; }
        public long BloodComponentTypeId { get; set; }
        public virtual BloodComponentType BloodComponentType { get; set; }
        public long BloodGroupMasterId { get; set; }
        public virtual BloodGroupMaster BloodGroupMaster { get; set; }
        public decimal Volume { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public long? BloodFridgeId { get; set; }
        public virtual BloodFridge BloodFridge { get; set; }
        public long? BloodRackId { get; set; }
        public virtual BloodRack BloodRack { get; set; }
        public string SlotNo { get; set; }
        public int Status { get; set; }
    }
}

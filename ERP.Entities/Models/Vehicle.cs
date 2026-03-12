namespace ERP.Entities.Models
{
    public class Vehicle : BaseEntity
    {
        public string VehicleName { get; set; }
        public string RegistrationNumber { get; set; }
        public string DriverName { get; set; }
        public string DriverPhoneNo { get; set; }
        public bool? IsHeadOfficeVehicle { get; set; }
        public string LogisticPartner { get; set; }
        public decimal LoadCapacity { get; set; }

        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }
    }
}

using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetVehicle
    {
        public long Id { get; set; }
        public string VehicleName { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal LoadCapacity { get; set; }
        public string DriverName { get; set; }
        public string DriverPhoneNo { get; set; }
        public bool? IsHeadOfficeVehicle { get; set; }
        public string LogisticPartner { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? DealershipId { get; set; }
        public GetDealership Dealership { get; set; }
    }
}

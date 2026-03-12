using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Command
{
    public class SaveVehicleCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string VehicleName { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal LoadCapacity { get; set; }
        public string DriverName { get; set; }
        public string DriverPhoneNo { get; set; }
        public bool? IsHeadOfficeVehicle { get; set; }
        public long? DealershipId { get; set; }
        public string LogisticPartner { get; set; }
    }
}

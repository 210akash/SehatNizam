using MediatR;

namespace ERP.Mediator.Mediator.Device.Command
{
    public class SaveDeviceCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string PhoneNo { get; set; }

        public int Port { get; set; }
        public bool IsActive { get; set; }

        public string IPAddress { get; set; }

        public string ConnectionStatus { get; set; }
    }
}

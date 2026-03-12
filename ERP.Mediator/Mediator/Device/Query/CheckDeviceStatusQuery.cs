using MediatR;

namespace ERP.Mediator.Mediator.Device.Query
{
    public class CheckDeviceStatusQuery : IRequest<bool>
    {
        public CheckDeviceStatusQuery(string IpAdress, int Port)
        {
            this.IpAdress = IpAdress;
            this.Port = Port;
        }

        public string IpAdress { get; set; }
        public int Port { get; set; }
    }
}
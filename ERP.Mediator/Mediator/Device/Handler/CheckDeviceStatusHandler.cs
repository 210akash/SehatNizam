using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Device.Query;
using ERP.Services.Helper;
using MediatR;

namespace ERP.Mediator.Mediator.Device.Handler
{
    public class CheckDeviceStatusHandler : IRequestHandler<CheckDeviceStatusQuery, bool>
    {
        private readonly ZkemClient zkemClient;

        public CheckDeviceStatusHandler(ZkemClient zkemClient)
        {
            this.zkemClient = zkemClient;
        }

        //public async Task<bool> Handle(CheckDeviceStatusQuery request, CancellationToken cancellationToken)
        //{
        //    return zkemClient.Connect_Net(request.IpAdress, request.Port);
        //}

        public Task<bool> Handle(CheckDeviceStatusQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

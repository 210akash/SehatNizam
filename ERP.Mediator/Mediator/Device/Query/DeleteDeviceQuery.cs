using MediatR;

namespace ERP.Mediator.Mediator.Device.Query
{
    public class DeleteDeviceQuery : IRequest<bool>
    {
        public DeleteDeviceQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Command
{
    public class SaveShipmentModeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}

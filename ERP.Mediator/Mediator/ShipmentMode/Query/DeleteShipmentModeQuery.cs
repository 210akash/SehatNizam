using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Query
{
    public class DeleteShipmentModeQuery : IRequest<bool>
    {
        public DeleteShipmentModeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
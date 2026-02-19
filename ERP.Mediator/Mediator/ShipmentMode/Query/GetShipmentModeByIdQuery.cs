using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Query
{
    public class GetShipmentModeByIdQuery : IRequest<GetShipmentMode>
    {
        public GetShipmentModeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Query
{
    public class GetVehicleByIdQuery : IRequest<GetVehicle>
    {
        public GetVehicleByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
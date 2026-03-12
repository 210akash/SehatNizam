using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Query
{
    public class GetVehiclesByDealershipQuery : IRequest<List<GetVehicle>>
    {
        public GetVehiclesByDealershipQuery(long DealershipId)
        {
            this.DealershipId = DealershipId;
        }

        public long DealershipId { get; set; }
    }
}
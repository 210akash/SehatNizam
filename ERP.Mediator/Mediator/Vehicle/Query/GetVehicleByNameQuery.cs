using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Query
{
    public class GetVehicleByNameQuery : IRequest<List<GetVehicle>>
    {
        public GetVehicleByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
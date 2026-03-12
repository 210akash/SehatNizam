using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Query
{
    public class GetShipmentModeByNameQuery : IRequest<List<GetShipmentMode>>
    {
        public GetShipmentModeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}
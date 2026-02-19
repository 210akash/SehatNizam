using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Query
{
    public class GetAllShipmentModeQuery : IRequest<Tuple<IEnumerable<GetShipmentMode>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
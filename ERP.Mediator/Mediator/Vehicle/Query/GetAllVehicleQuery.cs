using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Query
{
    public class GetAllVehicleQuery : IRequest<Tuple<IEnumerable<GetVehicle>, long>>
    {
        public long? RegionId { get; set; }
        public long? ZoneId { get; set; }
        public long? AreaId { get; set; }
        public long? TerritoryId { get; set; }
        public long? DealershipId { get; set; }

        public PagingData PagingData { get; set; }
    }
}
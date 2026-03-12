using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Device.Query
{
    public class GetAllDeviceQuery : IRequest<Tuple<IEnumerable<GetDevice>, long>>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public PagingData PagingData { get; set; }
    }
}
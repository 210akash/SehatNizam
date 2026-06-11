using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.ServiceType.Query
{
    public class GetAllServiceTypesQuery : IRequest<Tuple<IEnumerable<GetServiceType>, long>>
    {
        public string Name { get; set; }
        public PagingData PagingData { get; set; }

    }
}

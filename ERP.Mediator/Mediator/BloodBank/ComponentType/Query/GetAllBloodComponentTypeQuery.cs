using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.ComponentType.Query
{
    public class GetAllBloodComponentTypeQuery : IRequest<Tuple<IEnumerable<GetBloodComponentType>, long>>
    {
        public string Name { get; set; }
        public PagingData PagingData { get; set; }
    }
}

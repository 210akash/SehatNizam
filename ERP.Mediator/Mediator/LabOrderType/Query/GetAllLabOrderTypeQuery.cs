using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrderType.Query
{
    public class GetAllLabOrderTypeQuery : IRequest<Tuple<IEnumerable<GetLabOrderType>, long>>
    {
        public string Name { get; set; }
        public PagingData PagingData { get; set; }
    }
}

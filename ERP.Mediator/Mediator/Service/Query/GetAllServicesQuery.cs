using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Query
{
    public class GetAllServicesQuery : IRequest<Tuple<IEnumerable<GetService>, long>>
    {
        public long? ServiceTypeId { get; set; }
        public long? DepartmentId { get; set; }
        public string Name { get; set; }
        public bool? IsSurgical { get; set; }
        public PagingData PagingData { get; set; }

    }
}

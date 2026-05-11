using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RadiologyType.Query
{
    public class GetAllRadiologyTypesQuery : IRequest<Tuple<IEnumerable<GetRadiologyType>, long>>
    {
        public long? ServiceId { get; set; }
        public PagingData PagingData { get; set; }
    }
}

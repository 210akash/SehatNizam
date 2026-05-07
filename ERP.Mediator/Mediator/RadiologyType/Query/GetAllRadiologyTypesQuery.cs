using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Query
{
    public class GetAllRadiologyTypesQuery : IRequest<IEnumerable<GetRadiologyType>>
    {
        public long? ServiceId { get; set; }
    }
}

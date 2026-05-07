using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Query
{
    public class GetAllServicesQuery : IRequest<IEnumerable<GetService>>
    {
        public long? DepartmentId { get; set; }
    }
}

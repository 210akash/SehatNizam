using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetPendingOrderQuery : IRequest<List<GetOrder>>
    {
        public List<long> OrderId { get; set; }
        public string searchParam { get; set; }
    }
}
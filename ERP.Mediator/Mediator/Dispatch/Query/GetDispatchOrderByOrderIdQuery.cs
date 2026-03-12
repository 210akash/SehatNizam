using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetDispatchOrderByOrderIdQuery : IRequest<List<GetDispatchOrder>>
    {
        public GetDispatchOrderByOrderIdQuery(long OrderId)
        {
            this.OrderId = OrderId;
        }

        public long OrderId { get; set; }
    }
}
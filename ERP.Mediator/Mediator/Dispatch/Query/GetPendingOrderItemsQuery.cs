using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetPendingOrderItemsQuery : IRequest<List<GetOrderItems>>
    {
        public GetPendingOrderItemsQuery(long OrderId, long DispatchId)
        {
            this.OrderId = OrderId;
            this.DispatchId = DispatchId;
        }

        public long OrderId { get; set; }
        public long DispatchId { get; set; }
    }
}
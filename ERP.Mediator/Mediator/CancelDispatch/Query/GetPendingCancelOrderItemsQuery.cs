using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.CancelDispatch.Query
{
    public class GetPendingCancelOrderItemsQuery : IRequest<List<GetOrderItems>>
    {
        public GetPendingCancelOrderItemsQuery(long OrderId, long CancelDispatchId)
        {
            this.OrderId = OrderId;
            this.CancelDispatchId = CancelDispatchId;
        }

        public long OrderId { get; set; }
        public long CancelDispatchId { get; set; }
    }
}

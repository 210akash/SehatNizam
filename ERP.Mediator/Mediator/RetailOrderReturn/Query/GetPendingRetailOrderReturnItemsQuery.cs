using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Query
{
    public class GetPendingRetailOrderItemsQuery : IRequest<List<GetRetailOrderItems>>
    {
        public GetPendingRetailOrderItemsQuery(long RetailOrderId, long RetailOrderReturnId)
        {
            this.RetailOrderId = RetailOrderId;
            this.RetailOrderReturnId = RetailOrderReturnId;
        }

        public long RetailOrderId { get; set; }
        public long RetailOrderReturnId { get; set; }
    }
}
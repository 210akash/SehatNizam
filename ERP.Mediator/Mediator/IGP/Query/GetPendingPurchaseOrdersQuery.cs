using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class GetPendingPurchaseOrdersQuery : IRequest<List<GetDropDown>>
    {
        public GetPendingPurchaseOrdersQuery(long PurchaseOrderId)
        {
            this.PurchaseOrderId = PurchaseOrderId;
        }

        public long PurchaseOrderId { get; set; }
    }
}
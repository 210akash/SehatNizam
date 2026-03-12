using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopDispatch.Query
{
    public class GetPendingShopOrderItemsForDispatchQuery : IRequest<List<GetShopOrderItems>>
    {
        public GetPendingShopOrderItemsForDispatchQuery(long ShopOrderId, long ShopDispatchId,long DealershipId)
        {
            this.ShopOrderId = ShopOrderId;
            this.ShopDispatchId = ShopDispatchId;
            this.DealershipId = DealershipId;
        }

        public long ShopOrderId { get; set; }
        public long ShopDispatchId { get; set; }
        public long DealershipId { get; set; }
    }
}
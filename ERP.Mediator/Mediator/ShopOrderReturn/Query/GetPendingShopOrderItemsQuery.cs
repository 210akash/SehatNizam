using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Query
{
    public class GetPendingShopOrderItemsQuery : IRequest<List<GetShopOrderItems>>
    {
        public GetPendingShopOrderItemsQuery(long ShopOrderId, long ShopOrderReturnId)
        {
            this.ShopOrderId = ShopOrderId;
            this.ShopOrderReturnId = ShopOrderReturnId;
        }

        public long ShopOrderId { get; set; }
        public long ShopOrderReturnId { get; set; }
    }
}
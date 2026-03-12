using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Query
{
    public class GetPendingShopOrderQuery : IRequest<List<GetShopOrder>>
    {
        public GetPendingShopOrderQuery(long ShopOrderId, string searchParam)
        {
            this.ShopOrderId = ShopOrderId;
            this.searchParam = searchParam; 
        }

        public long ShopOrderId { get; set; }
        public string searchParam { get; set; }
    }
}
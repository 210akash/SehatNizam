using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopDispatch.Query
{
    public class GetPendingShopOrderForDispatchQuery : IRequest<List<GetShopOrder>>
    {
        public List<long> ShopOrderId { get; set; }
        public string searchParam { get; set; }
    }
}
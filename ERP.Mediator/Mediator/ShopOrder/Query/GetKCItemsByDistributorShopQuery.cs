using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class GetKCItemsByDistributorShopQuery : IRequest<List<GetItemStock>>
    {
        public GetKCItemsByDistributorShopQuery(long DistributorId)
        {
            this.DistributorId = DistributorId;
        }

        public long DistributorId { get; set; }
    }
}

using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class GetShopsByRouteIdQuery : IRequest<List<GetShop>>
    {
        public GetShopsByRouteIdQuery(long RouteId)
        {
            this.RouteId = RouteId;
        }

        public long RouteId { get; set; }
    }
}
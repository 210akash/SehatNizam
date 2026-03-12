using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class IsShopOccupiedQuery : IRequest<GetShopOccupied>
    {
        public IsShopOccupiedQuery(long ShopId, long RouteId)
        {
            this.ShopId = ShopId;
            this.RouteId = RouteId;
        }

        public long ShopId { get; set; }
        public long RouteId { get; set; }
    }
}
using MediatR;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class DeleteRouteShopQuery : IRequest<long>
    {
        public DeleteRouteShopQuery(long RouteShopId)
        {
            this.RouteShopId = RouteShopId;
        }

        public long RouteShopId { get; set; }
    }
}
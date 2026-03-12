using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class GetRouteByIdQuery : IRequest<GetRoute>
    {
        public GetRouteByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
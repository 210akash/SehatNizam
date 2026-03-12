using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Route.Command
{
    public class AddShopsRouteCommand : IRequest<long>
    {
        public GetRoute Route { get; set; }
        public List<GetShop> ShopsToAdd { get; set; }
    }
}

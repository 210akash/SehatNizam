using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.DSF.Command
{
    public class AddDSFRouteCommand : IRequest<long>
    {
        public GetUsers DSF { get; set; }
        public List<GetRoute> RoutesToAdd { get; set; }
    }
}

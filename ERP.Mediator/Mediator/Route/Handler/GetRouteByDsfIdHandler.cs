using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class GetRouteByDsfIdHandler : IRequestHandler<GetRoutesByDsfIdQuery, List<GetRoute>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRouteByDsfIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoute>> Handle(GetRoutesByDsfIdQuery request, CancellationToken cancellationToken)
        {
            List<GetRoute> getRoutesByDsfId = new List<GetRoute>();

            var routeList = await unitOfWork.Repository<DSFRoute>().GetAsync(x => x.DSFId == new System.Guid(request.DsfId) && x.IsActive == true);
            foreach (var item in routeList)
            {
                var route = await unitOfWork.Repository<Entities.Models.Route>().GetFirstAsNoTrackingAsync(x => x.Id == item.RouteId && x.TerritoryId == request.TerritoryId, null, null, "ShopRouteFrequency");
                var _route = mapper.Map<GetRoute>(route);
                getRoutesByDsfId.Add(_route);
            }

            return getRoutesByDsfId;
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class GetRouteByDSFTerritoryHandler : IRequestHandler<GetRouteByDSFTerritoryQuery, List<GetRoute>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRouteByDSFTerritoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoute>> Handle(GetRouteByDSFTerritoryQuery request, CancellationToken cancellationToken)
        {
            var dsfTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsync(y => y.UserId == new System.Guid(request.DsfId) && y.IsActive == true);

            var route = await unitOfWork.Repository<Entities.Models.Route>().GetAsync(y => y.TerritoryId == dsfTerritory.TerritoryId && y.IsActive == true, null, null, "Territory,RouteShop,ShopRouteFrequency");
            var _route = mapper.Map<List<GetRoute>>(route);
            return _route;
        }
    }
}

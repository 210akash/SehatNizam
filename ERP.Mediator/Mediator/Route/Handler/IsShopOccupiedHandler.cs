using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class IsShopOccupiedHandler : IRequestHandler<IsShopOccupiedQuery, GetShopOccupied>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public IsShopOccupiedHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetShopOccupied> Handle(IsShopOccupiedQuery request, CancellationToken cancellationToken)
        {
            var shopRoutes = unitOfWork.Repository<Entities.Models.RouteShop>().GetAsync(y => y.ShopId == request.ShopId && y.Route.IsActive == true && y.RouteId != request.RouteId && y.IsActive == true, null, null, "Route").Result.Select(x => x.RouteId);
            GetShopOccupied getShopOccupied = new GetShopOccupied();

            if(shopRoutes.Count() > 0)
            {
                var routes = await unitOfWork.Repository<Entities.Models.Route>().GetAsync(y => shopRoutes.Contains(y.Id));
                var _mapper = mapper.Map<List<GetRoute>>(routes);
                getShopOccupied.IsShopOccupied = true;
                getShopOccupied.RoutesInformation = _mapper;
            }
            else
            {
                getShopOccupied.IsShopOccupied = false;
                getShopOccupied.RoutesInformation = null;
            }
            return getShopOccupied;
        }
    }
}

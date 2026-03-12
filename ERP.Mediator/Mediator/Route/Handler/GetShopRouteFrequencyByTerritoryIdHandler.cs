using System.Collections.Generic;
using System.Linq;
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
    public class GetShopRouteFrequencyByTerritoryIdHandler : IRequestHandler<GetShopRouteFrequencyByTerritoryIdQuery, List<GetShopRouteFrequency>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopRouteFrequencyByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShopRouteFrequency>> Handle(GetShopRouteFrequencyByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
            var shopRouteFrequency = unitOfWork.Repository<ShopRouteFrequency>().GetAsync(y => y.Shop.TerritoryId == request.TerritoryId && y.IsActive == true).Result.ToList();
            var _shopRouteFrequency = mapper.Map<List<GetShopRouteFrequency>>(shopRouteFrequency);
            return _shopRouteFrequency;
        }
    }
}

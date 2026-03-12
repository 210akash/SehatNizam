using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class SearchShopsByTerritoryIdHandler : IRequestHandler<SearchShopsByTerritoryIdQuery, List<GetShopBasic>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SearchShopsByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShopBasic>> Handle(SearchShopsByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
            var shops = unitOfWork.Repository<Entities.Models.Shop>().GetAsync(y => y.IsActive == true && y.TerritoryId == request.TerritoryId && y.Name.ToLower().Contains(request.Param.ToLower()), null, null, "Territory,Territory.Area,Territory.Area.Zone").Result.Take(10);
            var dealershipId = unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y => y.IsActive == true && y.TerritoryId == request.TerritoryId).Result.Id;
            var _shops = mapper.Map<List<GetShopBasic>>(shops);

            foreach (var item in _shops)
            {
                item.DealershipId = dealershipId;
            }
            return _shops;
        }
    }
}

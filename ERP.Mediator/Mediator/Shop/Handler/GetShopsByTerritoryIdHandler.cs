using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class GetShopsByTerritoryIdHandler : IRequestHandler<GetShopsByTerritoryIdQuery, List<GetShop>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopsByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShop>> Handle(GetShopsByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
            var shops = await unitOfWork.Repository<Entities.Models.Shop>().GetAsync(y => y.IsActive == true && y.TerritoryId == request.TerritoryId, null, null, "Territory,Territory.Area,Territory.Area.Zone");
            var _shops = mapper.Map<List<GetShop>>(shops);
            return _shops;
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Handler
{
    public class GetTerritoryBySaleModelHandler : IRequestHandler<GetTerritoryBySaleModelQuery, List<GetTerritory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTerritoryBySaleModelHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetTerritory>> Handle(GetTerritoryBySaleModelQuery request, CancellationToken cancellationToken)
        {
            var territory = await unitOfWork.Repository<Entities.Models.Territory>().GetAsync(y => y.AreaId == request.AreaId && y.SaleModel == request.SaleModel && y.IsActive == true, null, null, "Shop,Dealership");
            var _territory = mapper.Map<List<GetTerritory>>(territory);
            return _territory;
        }
    }
}

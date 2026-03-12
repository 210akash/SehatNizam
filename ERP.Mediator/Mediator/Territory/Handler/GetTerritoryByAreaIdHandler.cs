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
    public class GetTerritoryByAreaIdHandler : IRequestHandler<GetTerritoryByAreaIdQuery, List<GetTerritory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTerritoryByAreaIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetTerritory>> Handle(GetTerritoryByAreaIdQuery request, CancellationToken cancellationToken)
        {
            var territory = await unitOfWork.Repository<Entities.Models.Territory>().GetAsync(y => y.AreaId == request.AreaId && y.IsActive == true, null, null, "Shop,Dealership");
            var _territory = mapper.Map<List<GetTerritory>>(territory);
            return _territory;
        }
    }
}

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
    public class GetTerritoryByNameHandler : IRequestHandler<GetTerritoryByNameQuery, List<GetTerritory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTerritoryByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetTerritory>> Handle(GetTerritoryByNameQuery request, CancellationToken cancellationToken)
        {
            var territory = await unitOfWork.Repository<Entities.Models.Territory>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _territory = mapper.Map<List<GetTerritory>>(territory);
            return _territory;
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Handler
{
    public class GetTerritoryByIdHandler : IRequestHandler<GetTerritoryByIdQuery, GetTerritory>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTerritoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetTerritory> Handle(GetTerritoryByIdQuery request, CancellationToken cancellationToken)
        {
            var territory = await unitOfWork.Repository<Entities.Models.Territory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _territory = mapper.Map<GetTerritory>(territory);
            return _territory;
        }
    }
}

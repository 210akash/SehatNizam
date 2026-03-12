using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UserTerritory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetUserTerritoryByIdHandler : IRequestHandler<GetUserTerritoryByIdQuery, GetUserTerritory>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetUserTerritoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetUserTerritory> Handle(GetUserTerritoryByIdQuery request, CancellationToken cancellationToken)
        {
            var UserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _UserTerritory = mapper.Map<GetUserTerritory>(UserTerritory);
            return _UserTerritory;
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UserTerritory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetUserTerritoryByNameHandler : IRequestHandler<GetUserTerritoryByNameQuery, List<GetUserTerritory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetUserTerritoryByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetUserTerritory>> Handle(GetUserTerritoryByNameQuery request, CancellationToken cancellationToken)
        {
            var UserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetAsync(y => y.IsActive);
            var _UserTerritory = mapper.Map<List<GetUserTerritory>>(UserTerritory);
            return _UserTerritory;
        }
    }
}

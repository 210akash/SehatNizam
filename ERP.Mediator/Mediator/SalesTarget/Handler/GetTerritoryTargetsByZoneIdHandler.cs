using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class GetTerritoryTargetsByZoneIdHandler : IRequestHandler<GetTerritoryTargetsByZoneIdQuery, List<GetSalesTarget>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTerritoryTargetsByZoneIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetSalesTarget>> Handle(GetTerritoryTargetsByZoneIdQuery request, CancellationToken cancellationToken)
        {
            var territoryTargetsByZoneId = await unitOfWork.Repository<Entities.Models.SalesTarget>().GetAsync(
                x => x.IsActive == true && x.UserId == null && x.TargetMonth.Month == request.TargetMonth.Month, null, null, "Territory");

            var _territoryTargetsByZoneId = mapper.Map<IEnumerable<GetSalesTarget>>(territoryTargetsByZoneId.ToList()).ToList();
            return _territoryTargetsByZoneId;
        }
    }
}

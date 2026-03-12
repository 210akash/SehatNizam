using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class GetDealershipByTerritorySaleModelHandler : IRequestHandler<GetDealershipByTerritorySaleModelQuery, GetDealership>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDealershipByTerritorySaleModelHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetDealership> Handle(GetDealershipByTerritorySaleModelQuery request, CancellationToken cancellationToken)
        {
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsync(y => y.DealershipTypeId == 1 && y.Territory.SaleModel == request.SaleModel && y.TerritoryId == request.TerritoryId && y.IsActive == true, null, null, "Territory");
            var _dealership = mapper.Map<GetDealership>(dealership);
            return _dealership;
        }
    }
}

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
    public class GetDealershipByTerritoryIdHandler : IRequestHandler<GetDealershipByTerritoryIdQuery, List<GetDealership>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDealershipByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDealership>> Handle(GetDealershipByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetAsync(y => y.DealershipTypeId == 1 && y.TerritoryId == request.TerritoryId && y.IsActive == true, null, null, "Territory");
            var _dealership = mapper.Map<List<GetDealership>>(dealership);
            return _dealership;
        }
    }
}

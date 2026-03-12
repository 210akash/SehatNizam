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
    public class GetDealershipByNameHandler : IRequestHandler<GetDealershipByNameQuery, List<GetDealership>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDealershipByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDealership>> Handle(GetDealershipByNameQuery request, CancellationToken cancellationToken)
        {
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetAsync(y => y.DealershipTypeId == 1 && y.Name.ToLower().Contains(request.name));
            var _dealership = mapper.Map<List<GetDealership>>(dealership);
            return _dealership;
        }
    }
}

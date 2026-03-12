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
    public class GetCustomerByNameHandler : IRequestHandler<GetCustomerByNameQuery, List<GetDealership>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCustomerByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDealership>> Handle(GetCustomerByNameQuery request, CancellationToken cancellationToken)
        {
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetAsync(y => y.DealershipTypeId == 2 && y.Name.ToLower().Contains(request.name));
            var _dealership = mapper.Map<List<GetDealership>>(dealership);
            return _dealership;
        }
    }
}

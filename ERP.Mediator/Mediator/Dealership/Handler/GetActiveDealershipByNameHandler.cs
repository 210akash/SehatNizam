using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class GetActiveDealershipByNameHandler : IRequestHandler<GetActiveDealershipByNameQuery, List<GetDealership>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetActiveDealershipByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDealership>> Handle(GetActiveDealershipByNameQuery request, CancellationToken cancellationToken)
        {
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetAsync(y => (y.DealershipTypeId == 1 || y.DealershipTypeId == 3 || y.DealershipTypeId == 4) && y.Name.ToLower().Contains(request.name), null, null, "Territory,DealershipType");
            var _dealership = mapper.Map<List<GetDealership>>(dealership);
            return _dealership;

            //var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetAsync(y => y.DealershipTypeId == 1 && y.IsActive == true && y.IsDelete == false && y.Name.ToLower().Contains(request.name),null,null,"Territory");
            //var _dealership = mapper.Map<List<GetDealership>>(dealership);
            //return _dealership;
        }
    }
}

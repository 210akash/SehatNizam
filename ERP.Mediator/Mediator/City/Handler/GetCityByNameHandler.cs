using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.City.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class GetCityByNameHandler : IRequestHandler<GetCityByNameQuery, List<GetCity>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCityByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetCity>> Handle(GetCityByNameQuery request, CancellationToken cancellationToken)
        {
            var city = await unitOfWork.Repository<Entities.Models.City>().GetAsync(y => y.Name == request.name);
            var _city = mapper.Map<List<GetCity>>(city);
            return _city;
        }
    }
}

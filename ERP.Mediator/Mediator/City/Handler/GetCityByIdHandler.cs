using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.City.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class GetCityByIdHandler : IRequestHandler<GetCityByIdQuery, GetCity>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCityByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetCity> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var city = await unitOfWork.Repository<Entities.Models.City>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _city = mapper.Map<GetCity>(city);
            return _city;
        }
    }
}

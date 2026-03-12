using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Vehicle.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Handler
{
    public class GetVehiclesByDealershipHandler : IRequestHandler<GetVehiclesByDealershipQuery, List<GetVehicle>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetVehiclesByDealershipHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetVehicle>> Handle(GetVehiclesByDealershipQuery request, CancellationToken cancellationToken)
        {
            if (request.DealershipId == 0)
            {
                var headOfficeVehicles = await unitOfWork.Repository<Entities.Models.Vehicle>().FindAllAsync(x => x.IsHeadOfficeVehicle == true && x.IsActive == true);
                var mapHeadOfficeVehicles = mapper.Map<List<GetVehicle>>(headOfficeVehicles);
                return mapHeadOfficeVehicles;
            }
            else
            {
                var dealerVehicles = await unitOfWork.Repository<Entities.Models.Vehicle>().FindAllAsync(x => x.DealershipId == request.DealershipId && x.IsActive == true);
                var mapDealerVehicles = mapper.Map<List<GetVehicle>>(dealerVehicles);
                return mapDealerVehicles;
            }
        }
    }
}

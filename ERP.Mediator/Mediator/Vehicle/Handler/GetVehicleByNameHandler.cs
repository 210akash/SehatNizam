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
    public class GetVehicleByNameHandler : IRequestHandler<GetVehicleByNameQuery, List<GetVehicle>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetVehicleByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetVehicle>> Handle(GetVehicleByNameQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await unitOfWork.Repository<Entities.Models.Vehicle>().GetAsync(y => 
            y.VehicleName.ToLower().Contains(request.name.ToLower()) ||
            y.RegistrationNumber.ToLower().Contains(request.name.ToLower())
            );

            var _vehicle = mapper.Map<List<GetVehicle>>(vehicle);
            return _vehicle;
        }
    }
}

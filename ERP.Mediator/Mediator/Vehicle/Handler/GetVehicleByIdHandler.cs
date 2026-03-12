using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Vehicle.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Handler
{
    public class GetVehicleByIdHandler : IRequestHandler<GetVehicleByIdQuery, GetVehicle>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetVehicleByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetVehicle> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await unitOfWork.Repository<Entities.Models.Vehicle>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _vehicle = mapper.Map<GetVehicle>(vehicle);
            return _vehicle;
        }
    }
}

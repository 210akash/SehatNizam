using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShipmentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetShipmentModeByIdQuery, GetShipmentMode>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetShipmentMode> Handle(GetShipmentModeByIdQuery request, CancellationToken cancellationToken)
        {
            var ShipmentMode = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _ShipmentMode = mapper.Map<GetShipmentMode>(ShipmentMode);
            return _ShipmentMode;
        }
    }
}

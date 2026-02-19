using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShipmentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetShipmentModeByNameQuery, List<GetShipmentMode>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShipmentMode>> Handle(GetShipmentModeByNameQuery request, CancellationToken cancellationToken)
        {
            var ShipmentMode = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetAsync(y => y.Name == request.name);
            var _ShipmentMode = mapper.Map<List<GetShipmentMode>>(ShipmentMode);
            return _ShipmentMode;
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShipmentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetShipmentModeByCompanyQuery, List<GetShipmentMode>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetShipmentMode>> Handle(GetShipmentModeByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var ShipmentMode = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _ShipmentMode = mapper.Map<List<GetShipmentMode>>(ShipmentMode);
                return _ShipmentMode;
            }
            else
            {
                var ShipmentMode = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _ShipmentMode = mapper.Map<List<GetShipmentMode>>(ShipmentMode);
                return _ShipmentMode;
            }
        }
    }
}

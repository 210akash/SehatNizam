using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class GetZoneByIdHandler : IRequestHandler<GetZoneByIdQuery, GetZone>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetZoneByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetZone> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
        {
            var zone = await unitOfWork.Repository<Entities.Models.Zone>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _zone = mapper.Map<GetZone>(zone);
            return _zone;
        }
    }
}

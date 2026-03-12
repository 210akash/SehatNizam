using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class GetZoneByNameHandler : IRequestHandler<GetZoneByNameQuery, List<GetZone>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetZoneByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetZone>> Handle(GetZoneByNameQuery request, CancellationToken cancellationToken)
        {
            var zone = await unitOfWork.Repository<Entities.Models.Zone>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _zone = mapper.Map<List<GetZone>>(zone);
            return _zone;
        }
    }
}

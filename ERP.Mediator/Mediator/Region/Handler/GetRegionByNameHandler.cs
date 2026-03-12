using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Region.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Handler
{
    public class GetRegionByNameHandler : IRequestHandler<GetRegionByNameQuery, List<GetRegion>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRegionByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRegion>> Handle(GetRegionByNameQuery request, CancellationToken cancellationToken)
        {
            var region = await unitOfWork.Repository<Entities.Models.Region>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _region = mapper.Map<List<GetRegion>>(region);
            return _region;
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Region.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Handler
{
    public class GetRegionByIdHandler : IRequestHandler<GetRegionByIdQuery, GetRegion>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRegionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRegion> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
        {
            var region = await unitOfWork.Repository<Entities.Models.Region>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _region = mapper.Map<GetRegion>(region);
            return _region;
        }
    }
}

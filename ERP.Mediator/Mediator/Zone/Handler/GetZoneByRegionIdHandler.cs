using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class GetZoneByRegionIdHandler : IRequestHandler<GetZoneByRegionIdQuery, List<GetZone>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetZoneByRegionIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetZone>> Handle(GetZoneByRegionIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Zone, bool>> predicate = y => y.RegionId == request.RegionId && y.IsActive == true;

            Expression<Func<Entities.Models.Zone, object>>[] includes = { x => x.Area };

            var entity = unitOfWork.Repository<Entities.Models.Zone>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, includes);


            //var zone = await unitOfWork.Repository<Entities.Models.Zone>().GetAsync(y => y.RegionId == request.RegionId && y.IsActive == true,null,null, includes);
            var _zone = mapper.Map<List<GetZone>>(entity.Item1.ToList());
            return _zone;
        }
    }
}

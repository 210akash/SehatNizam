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
    public class GetAllZoneHandler : IRequestHandler<GetAllZoneQuery, Tuple<IEnumerable<GetZone>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllZoneHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetZone>, long>> Handle(GetAllZoneQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Zone, bool>> predicate = x => x.IsActive == true
            && (request.RegionId == 0 || x.RegionId == request.RegionId)
            ;

            Expression<Func<Entities.Models.Zone, object>>[] includes = {
                //x => x.Territory,
                x => x.Region
            };

            Expression<Func<Entities.Models.Zone, object>> OrderBy = null;
            Expression<Func<Entities.Models.Zone, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Zone>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var zone = mapper.Map<IEnumerable<GetZone>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetZone>, long>(zone, entity.Item2);
        }
    }
}

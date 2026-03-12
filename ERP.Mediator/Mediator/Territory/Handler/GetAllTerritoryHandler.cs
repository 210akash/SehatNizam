using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Handler
{
    public class GetAllTerritoryHandler : IRequestHandler<GetAllTerritoryQuery, Tuple<IEnumerable<GetTerritory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllTerritoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetTerritory>, long>> Handle(GetAllTerritoryQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Territory, bool>> predicate = x => x.IsActive == true
            && (request.RegionId == 0 || x.Area.Zone.RegionId == request.RegionId)
            && (request.ZoneId == 0 || x.Area.ZoneId == request.ZoneId)
            && (request.AreaId == 0 || x.AreaId == request.AreaId)
            && x.Id != 11
            ;

            Expression<Func<Entities.Models.Territory, object>>[] includes = {
                x => x.Area,
                x => x.Area.Zone,
                x => x.Area.Zone.Region
            };

            Expression<Func<Entities.Models.Territory, object>> OrderBy = null;
            Expression<Func<Entities.Models.Territory, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Territory>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var territory = mapper.Map<IEnumerable<GetTerritory>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetTerritory>, long>(territory, entity.Item2);
        }
    }
}

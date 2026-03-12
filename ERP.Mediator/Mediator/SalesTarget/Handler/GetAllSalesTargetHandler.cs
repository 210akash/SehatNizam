using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class GetAllSalesTargetHandler : IRequestHandler<GetAllSalesTargetQuery, Tuple<IEnumerable<GetSalesTarget>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllSalesTargetHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetSalesTarget>, long>> Handle(GetAllSalesTargetQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.SalesTarget, bool>> predicate = x => x.IsActive == true
            && (request.RegionId == 0 || x.User.UserTerritory.Any(x => x.RegionId == request.RegionId))
            && (request.ZoneId == 0 || x.User.UserTerritory.Any(x => x.ZoneId == request.ZoneId))
            && (request.AreaId == 0 || x.User.UserTerritory.Any(x => x.AreaId == request.AreaId))
            && (request.TerritoryId == 0 || x.User.UserTerritory.Any(x => x.TerritoryId == request.TerritoryId))
            ;

            Expression<Func<Entities.Models.SalesTarget, object>>[] includes = {
                x => x.User,
                x => x.User.UserTerritory,
                //x => x.Zone.Territory,
            };

            List<string> thenInclude = new List<string>();
            thenInclude.Add("User.UserTerritory.Region");
            thenInclude.Add("User.UserTerritory.Zone");
            thenInclude.Add("User.UserTerritory.Area");
            thenInclude.Add("User.UserTerritory.Territory");

            Expression<Func<Entities.Models.SalesTarget, object>> OrderBy = null;
            Expression<Func<Entities.Models.SalesTarget, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.SalesTarget>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var salesTarget = mapper.Map<IEnumerable<GetSalesTarget>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetSalesTarget>, long>(salesTarget, entity.Item2);
        }
    }
}

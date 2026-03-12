using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.DSF.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DSF.Handler
{
    public class GetAllDSFHandler : IRequestHandler<GetAllDSFQuery, Tuple<IEnumerable<GetUsers>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllDSFHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetUsers>, long>> Handle(GetAllDSFQuery request, CancellationToken cancellationToken)
        {
            var DSFRoleId = unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "DSF").Result.Id;
            var SalesmanRoleId = unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "Salesman").Result.Id;

            Expression<Func<AspNetUsers, bool>> predicate = x => x.IsActive == true
            && x.AspNetUserRoles.Any(x => x.RoleId == DSFRoleId || x.RoleId == SalesmanRoleId)
            && (request.RegionId == 0 || x.UserTerritory.Any(y => y.Territory.Area.Zone.RegionId == request.RegionId && y.IsActive))
            && (request.ZoneId == 0 || x.UserTerritory.Any(y => y.Territory.Area.ZoneId == request.ZoneId && y.IsActive))
            && (request.AreaId == 0 || x.UserTerritory.Any(y => y.Territory.AreaId == request.AreaId && y.IsActive))
            && (request.TerritoryId == 0 || x.UserTerritory.Any(y => y.TerritoryId == request.TerritoryId && y.IsActive))
            ;

            Expression<Func<AspNetUsers, object>>[] includes = {
                x => x.AspNetUserRoles,
                x => x.DSFRoute,
                //x => x.Attachments
            };

            List<string> thenInclude = new List<string>();
            thenInclude.Add("DSFRoute.Route");
            thenInclude.Add("DSFRoute.Route.Territory");
            thenInclude.Add("DSFRoute.Route.Territory.Area");
            thenInclude.Add("DSFRoute.Route.Territory.Area.Zone");
            thenInclude.Add("DSFRoute.Route.Territory.Area.Zone.Region");
            thenInclude.Add("AspNetUserRoles.Role");
            thenInclude.Add("UserTerritory.Territory");
            thenInclude.Add("UserTerritory.Territory.Area");
            thenInclude.Add("UserTerritory.Territory.Area.Zone");
            thenInclude.Add("UserTerritory.Territory.Area.Zone.Region");

            Expression<Func<AspNetUsers, object>> OrderBy = null;
            Expression<Func<AspNetUsers, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var _entity = mapper.Map<IEnumerable<GetUsers>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetUsers>, long>(_entity, entity.Item2);
        }
    }
}

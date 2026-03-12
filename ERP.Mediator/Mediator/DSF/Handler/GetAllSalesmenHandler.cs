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
    public class GetAllSalesmenHandler : IRequestHandler<GetAllSalesmenQuery, Tuple<IEnumerable<GetUsers>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllSalesmenHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetUsers>, long>> Handle(GetAllSalesmenQuery request, CancellationToken cancellationToken)
        {
            var SalesmanRoleId = unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "Salesman").Result.Id;

            Expression<Func<AspNetUsers, bool>> predicate = x => x.IsActive == true && x.AspNetUserRoles.Any(x => x.RoleId == SalesmanRoleId)
            && (request.ZoneId == 0
            //|| x.UserTerritory.Any(y => y.Territory.ZoneId == request.ZoneId && y.IsActive)
            )
            && (request.TerritoryId == 0 || x.UserTerritory.Any(y => y.TerritoryId == request.TerritoryId && y.IsActive))
            ;

            Expression<Func<AspNetUsers, object>>[] includes = {
                x => x.AspNetUserRoles,
                x => x.DSFRoute,
                //x => x.Attachments,
                x => x.UserTerritory
            };

            List<string> thenInclude = new List<string>();
            thenInclude.Add("DSFRoute.Route");
            thenInclude.Add("DSFRoute.Route.Territory");
            thenInclude.Add("DSFRoute.Route.Territory.Area");
            thenInclude.Add("DSFRoute.Route.Territory.Area.Zone");
            thenInclude.Add("DSFRoute.Route.Territory.Zone");
            thenInclude.Add("AspNetUserRoles.Role");

            Expression<Func<AspNetUsers, object>> OrderBy = null;
            Expression<Func<AspNetUsers, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var zone = mapper.Map<IEnumerable<GetUsers>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetUsers>, long>(zone, entity.Item2);
        }
    }
}

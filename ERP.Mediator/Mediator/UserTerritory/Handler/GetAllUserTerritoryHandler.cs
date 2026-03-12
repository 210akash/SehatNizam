using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.UserTerritory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetAllUserTerritoryHandler : IRequestHandler<GetAllUserTerritoryQuery, Tuple<IEnumerable<GetUserTerritory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllUserTerritoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetUserTerritory>, long>> Handle(GetAllUserTerritoryQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.UserTerritory, bool>> predicate = x => x.IsActive == true
            && (request.RegionId == 0 || x.RegionId == request.RegionId)
            && (request.ZoneId == 0 || x.ZoneId == request.ZoneId)
            && (request.AreaId == 0 || x.AreaId == request.AreaId)
            && (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId)
            && x.UserId !=  new Guid("49EE1B31-79EE-4B2E-A79F-6B8CC1ADA449")
            && x.UserId !=  new Guid("01762419-1224-4822-A2CF-9AFB5E2C54E4")
            && x.UserId !=  new Guid("5E61F910-6E56-4163-BCBF-9ED42A4F12EF")
            && x.UserId !=  new Guid("56CA6B1D-E87C-40D4-8220-CA029D3791AD")
            && x.UserId !=  new Guid("C265A3F1-802D-4CC6-B3C4-FE0DB1AE0585")
            //HeadOffice User
            ;

            Expression<Func<Entities.Models.UserTerritory, object>>[] includes = {
                x => x.User,
                x => x.Zone,
                x => x.Territory,
                x => x.Region,
                x => x.Area,
                x => x.Shop,
                x => x.User.AspNetUserRoles
            };

            Expression<Func<Entities.Models.UserTerritory, object>> OrderBy = null;
            Expression<Func<Entities.Models.UserTerritory, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("User.AspNetUserRoles.Role");

            var entity = unitOfWork.Repository<Entities.Models.UserTerritory>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var UserTerritory = mapper.Map<IEnumerable<GetUserTerritory>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetUserTerritory>, long>(UserTerritory, entity.Item2);
        }
    }
}

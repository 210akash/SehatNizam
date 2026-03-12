using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.UserAttendance.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.UserAttendance.Handler
{
    public class GetAllUserAttendanceHandler : IRequestHandler<GetAllUserAttendanceQuery, Tuple<IEnumerable<GetUserAttendance>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IAuthService authService;

        public GetAllUserAttendanceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider, IAuthService authService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.authService = authService;
        }

        public async Task<Tuple<IEnumerable<GetUserAttendance>, long>> Handle(GetAllUserAttendanceQuery request, CancellationToken cancellationToken)
        {
            //string Role = authService.GetCurrentUserRole();
            Expression<Func<Entities.Models.UserAttendance, bool>> predicate;
            //if (Role == "ASE")
            //{
            //    var TerritoryId = sessionProvider.Session.TerritoryId;
            //    var activeTerritoryUsersIds = unitOfWork.Repository<Entities.Models.UserTerritory>()
            //                                   .GetAll()
            //                                   .Where(x => x.IsActive == true && x.TerritoryId == TerritoryId)
            //                                   .Select(y=>y.UserId)
            //                                   .Distinct()
            //                                   .ToList();

            //    predicate = x => x.IsActive == true && x.IsDelete == false && activeTerritoryUsersIds.Contains(x.UserId)
            //                && x.AttendanceDate >= request.FDate
            //                && x.AttendanceDate <= request.TDate.Value.AddDays(1).AddSeconds(-1);
            //}
            //else
            //{
            predicate = x => x.IsActive == true && x.AttendanceDate >= request.FDate && x.AttendanceDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
            && (!string.IsNullOrWhiteSpace(request.RoleId) ? x.User.AspNetUserRoles.Any(y => y.RoleId == new Guid(request.RoleId)) : true);
            //}

            Expression<Func<Entities.Models.UserAttendance, object>>[] includes = {
                x =>x.User,
                x => x.User.AspNetUserRoles,
                x =>x.Dealership,
                x =>x.Dealership.Territory,
                x =>x.Dealership.Territory.Area,
                x =>x.Dealership.Territory.Area.Zone,
                x =>x.Dealership.Territory.Area.Zone.Region
            };

            Expression<Func<Entities.Models.UserAttendance, object>> OrderBy = null;
            Expression<Func<Entities.Models.UserAttendance, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("User.AspNetUserRoles.Role");


            var entity = unitOfWork.Repository<Entities.Models.UserAttendance>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var UserAttendance = mapper.Map<IEnumerable<GetUserAttendance>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetUserAttendance>, long>(UserAttendance, entity.Item2);
        }
    }
}

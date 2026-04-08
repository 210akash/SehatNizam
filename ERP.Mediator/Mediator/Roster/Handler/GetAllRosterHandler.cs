using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Roster.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Roster.Handler
{
    public class GetAllRosterHandler : IRequestHandler<GetAllRosterQuery, Tuple<IEnumerable<GetRoster>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllRosterHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetRoster>, long>> Handle(GetAllRosterQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Roster, bool>> predicate;

            Expression<Func<Entities.Models.Roster, object>>[] includes = {
                x => x.Department,
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.RosterDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new()
            {
                "RosterDetail.Employee",
                "RosterDetail.EmployeeShift"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Hr Manager"))
            {
                predicate = x => x.IsActive == true && x.Department.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                      && x.Month >= request.Month
                      && x.Year <= request.Year
                      && (request.DepartmentId == null || x.DepartmentId == request.DepartmentId);
            }
            else
            {
                predicate = x => x.IsActive == true && x.Department.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                      && x.Month >= request.Month
                      && x.Year <= request.Year
                      && x.DepartmentId == this.sessionProvider.Session.DepartmentId;
            }

            Expression<Func<Entities.Models.Roster, object>> OrderBy = null;
            Expression<Func<Entities.Models.Roster, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Roster>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var Roster = mapper.Map<IEnumerable<GetRoster>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetRoster>, long>(Roster, entity.Item2);
        }
    }
}

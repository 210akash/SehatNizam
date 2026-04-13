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
    public class GetAllRosterByManagerHandler : IRequestHandler<GetAllRosterByManagerQuery, Tuple<IEnumerable<GetRoster>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllRosterByManagerHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetRoster>, long>> Handle(GetAllRosterByManagerQuery request, CancellationToken cancellationToken)
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

            predicate = x => x.IsActive == true && x.Department.CompanyId == this.sessionProvider.Session.CompanyId
                  && (request.StatusId == 0 || x.StatusId == request.StatusId)
                  && x.Year == request.Year
                  && x.Month == request.Month
                  && x.DepartmentId == this.sessionProvider.Session.DepartmentId;

            Expression<Func<Entities.Models.Roster, object>> OrderBy = null;
            Expression<Func<Entities.Models.Roster, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Roster>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var Roster = mapper.Map<IEnumerable<GetRoster>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetRoster>, long>(Roster, entity.Item2);
        }
    }
}

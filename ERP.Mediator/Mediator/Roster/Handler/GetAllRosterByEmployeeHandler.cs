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
    public class GetAllRosterByEmployeeHandler : IRequestHandler<GetAllRosterByEmployeeQuery, GetRoster>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllRosterByEmployeeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<GetRoster> Handle(GetAllRosterByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var loggedInUserId = this.sessionProvider.Session.LoggedInUserId;

            Expression<Func<Entities.Models.Roster, bool>> predicate;

            Expression<Func<Entities.Models.Roster, object>>[] includes = {
                x => x.Department,
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.RosterDetail.Where(y => y.IsActive == true && y.EmployeeId == loggedInUserId)  // Filter by logged-in employee
             };

            List<string> thenIncludes = new()
            {
                "RosterDetail.Employee",
                "RosterDetail.EmployeeShift"
            };

            predicate = x => x.IsActive == true && x.Department.CompanyId == this.sessionProvider.Session.CompanyId
                  && (x.StatusId == 3)
                  && x.Year == request.Year
                  && x.Month == request.Month
                  && x.RosterDetail.Any(r => r.EmployeeId == loggedInUserId);  // Only rosters containing this employee

            var entity = await unitOfWork.Repository<Entities.Models.Roster>().GetFirstAsync(predicate, null, null, "RosterDetail,RosterDetail.Employee,RosterDetail.EmployeeShift");
            var Roster = mapper.Map<GetRoster>(entity);
            return Roster;
        }
    }
}

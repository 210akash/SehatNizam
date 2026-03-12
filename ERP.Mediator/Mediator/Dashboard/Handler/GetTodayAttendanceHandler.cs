using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dashboard.Query;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dashboard.Handler
{
    public class GetTodayAttendanceHandler : IRequestHandler<GetTodayAttendanceQuery, GetTodayAttendance>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTodayAttendanceHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetTodayAttendance> Handle(GetTodayAttendanceQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;

            var employees = unitOfWork.Repository<AspNetUsers>().GetAsync(x => x.IsActive && !x.IsDelete && x.IsEmployee, null, null, "AspNetUserRoles,AspNetUserRoles.Role,Department").Result.Select(x => x.Id).ToList();
            var userAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetAsync(x => x.IsActive && !x.IsDelete && x.CreatedDate.Value.Date == today && employees.Contains((Guid)x.UserId));
            var employeeLeaves = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetAsync(x => x.IsActive && !x.IsDelete && today >= x.StartDate.Date && today <= x.EndDate.Date && employees.Contains(x.EmployeeId));

            var presentIds = userAttendance
                .Select(x => x.UserId.Value)
                .Distinct()
                .ToList();

            var leavesAppliedManager = employeeLeaves.Where(x => x.StatusId == 2)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            var leavesAppliedHR = employeeLeaves.Where(x => x.StatusId == 3)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            var onLeave = employeeLeaves.Where(x => x.StatusId == 160)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            var absentIds = employees
                .Where(id => !presentIds.Contains(id) && !leavesAppliedManager.Contains(id) && !leavesAppliedHR.Contains(id) && !onLeave.Contains(id))
                .ToList();

            GetTodayAttendance getTodayAttendance = new GetTodayAttendance();
            getTodayAttendance.Present = presentIds.Count();
            getTodayAttendance.Absent = absentIds.Count();
            getTodayAttendance.LeaveAppliedManager = leavesAppliedManager.Count();
            getTodayAttendance.LeaveAppliedHR = leavesAppliedHR.Count();
            getTodayAttendance.OnLeave = onLeave.Count();
            return getTodayAttendance;
        }
    }
}

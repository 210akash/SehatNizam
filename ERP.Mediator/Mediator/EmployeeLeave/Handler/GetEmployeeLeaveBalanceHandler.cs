using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class GetEmployeeLeaveBalanceHandler : IRequestHandler<GetEmployeeLeaveBalanceQuery, List<LeaveBalanceDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetEmployeeLeaveBalanceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<LeaveBalanceDto>> Handle(GetEmployeeLeaveBalanceQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;
            var hrYear = await unitOfWork.Repository<Entities.Models.HRYear>()
                .GetFirstAsNoTrackingAsync(y => y.IsActive && !y.IsDelete && DateTime.Now.Date >= y.StartDate.Date && DateTime.Now.Date <= y.EndDate.Date);

            var currentUser = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync
                (x => x.Id == sessionProvider.Session.LoggedInUserId, null, null,
                "EmployeeLeaveGroup," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType.HRYear," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail.EmployeeLeaveType"
                );

            var employeeLeaves = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetAsync
                (x => x.StatusId != 4 && x.EmployeeId == sessionProvider.Session.LoggedInUserId && x.IsActive && !x.IsDelete && x.CreatedDate.Value.Date >= hrYear.StartDate.Date && x.CreatedDate.Value.Date <= hrYear.EndDate.Date, null, null,
                "EmployeeGroupLeaveTypeDetail,"
                );

            var usedLeavesByType = employeeLeaves
                .GroupBy(l => l.EmployeeGroupLeaveTypeDetail.EmployeeLeaveTypeId)
                .Select(g => new
                {
                    LeaveTypeId = g.Key,
                    UsedDays = g.Sum(l =>
                        ((l.EndDate - l.StartDate).TotalDays + 1)
                        - (l.IsFirstHalfDay ? 0.5 : 0)
                        - (l.IsLastHalfDay ? 0.5 : 0)
                    )
                }).ToList();

            var leaveDetails = currentUser.EmployeeLeaveGroup.EmployeeGroupLeaveType.Where(x => today >= x.HRYear.StartDate.Date && today <= x.HRYear.EndDate.Date)
                .SelectMany(x => x.EmployeeGroupLeaveTypeDetail)
                .ToList();

            var leaveBalances = leaveDetails.Select(detail =>
            {
                var used = usedLeavesByType.FirstOrDefault(x => x.LeaveTypeId == detail.EmployeeLeaveTypeId)?.UsedDays ?? 0;
                return new LeaveBalanceDto
                {
                    Id = detail.Id,
                    LeaveType = detail.EmployeeLeaveType.Name,
                    Allotted = detail.NoOfLeaves,
                    Used = used,
                    Balance = detail.NoOfLeaves - used
                };
            }).ToList();

            return leaveBalances;
        }
    }
}

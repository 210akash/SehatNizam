using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class GetLeaveBalanceByEmployeeHandler : IRequestHandler<GetLeaveBalanceByEmployeeQuery, List<LeaveBalanceDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        public GetLeaveBalanceByEmployeeHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<LeaveBalanceDto>> Handle(GetLeaveBalanceByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;

            var hrYear = await unitOfWork.Repository<Entities.Models.HRYear>()
                .GetFirstAsNoTrackingAsync(y =>
                    y.IsActive && !y.IsDelete &&
                    today >= y.StartDate.Date &&
                    today <= y.EndDate.Date);

            var currentUser = await unitOfWork.Repository<Entities.Models.AspNetUsers>()
                .GetFirstAsNoTrackingAsync(
                    x => x.Id == request.EmployeeId,
                    null,
                    null,
                    "EmployeeLeaveGroup," +
                    "EmployeeLeaveGroup.EmployeeGroupLeaveType," +
                    "EmployeeLeaveGroup.EmployeeGroupLeaveType.HRYear," +
                    "EmployeeLeaveGroup.EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail," +
                    "EmployeeLeaveGroup.EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail.EmployeeLeaveType"
                );

            var leaveDetails = currentUser.EmployeeLeaveGroup.EmployeeGroupLeaveType
                .Where(x => today >= x.HRYear.StartDate.Date && today <= x.HRYear.EndDate.Date)
                .SelectMany(x => x.EmployeeGroupLeaveTypeDetail)
                .ToList();

            // 🚫 NOT CONFIRMED → NO LEAVE ALLOWED
            if (!currentUser.DateOfConfirmation.HasValue ||
                today < currentUser.DateOfConfirmation.Value.Date)
            {
                return leaveDetails.Select(detail => new LeaveBalanceDto
                {
                    Id = detail.Id,
                    LeaveType = detail.EmployeeLeaveType.Name,
                    Allotted = 0,
                    Used = 0,
                    Balance = 0
                }).ToList();
            }

            var employeeLeaves = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetAsync(
                x => x.StatusId != 4 &&
                     x.EmployeeId == request.EmployeeId &&
                     x.IsActive &&
                     !x.IsDelete &&
                     x.CreatedDate.Value.Date >= hrYear.StartDate.Date &&
                     x.CreatedDate.Value.Date <= hrYear.EndDate.Date,
                null,
                null,
                "EmployeeGroupLeaveTypeDetail"
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

            var leaveBalances = leaveDetails.Select(detail =>
            {
                double used = usedLeavesByType
                    .FirstOrDefault(x => x.LeaveTypeId == detail.EmployeeLeaveTypeId)
                    ?.UsedDays ?? 0;

                double proratedNoOfLeaves = detail.NoOfLeaves;

                // Prorate Annual Leave only
                if (detail.EmployeeLeaveType.Name == "Annual Leave" &&
                    currentUser.DateOfConfirmation >= hrYear.StartDate.Date &&
                    currentUser.DateOfConfirmation <= hrYear.EndDate.Date)
                {
                    int totalMonthsInYear = 12;

                    int monthsWorked =
                        (hrYear.EndDate.Year - currentUser.DateOfConfirmation.Value.Year) * 12
                        + hrYear.EndDate.Month
                        - currentUser.DateOfConfirmation.Value.Month
                        + 1;

                    proratedNoOfLeaves = (double)Math.Round(
                        (Convert.ToDecimal(detail.NoOfLeaves) / totalMonthsInYear) * monthsWorked,
                        2
                    );
                }

                return new LeaveBalanceDto
                {
                    Id = detail.Id,
                    LeaveType = detail.EmployeeLeaveType.Name,
                    Allotted = (decimal)proratedNoOfLeaves,
                    Used = used,
                    Balance = proratedNoOfLeaves - used
                };
            }).ToList();

            return leaveBalances;
        }

        public async Task<List<LeaveBalanceDto>> Handle_old(GetLeaveBalanceByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;
            var hrYear = await unitOfWork.Repository<Entities.Models.HRYear>()
                .GetFirstAsNoTrackingAsync(y => y.IsActive && !y.IsDelete && DateTime.Now.Date >= y.StartDate.Date && DateTime.Now.Date <= y.EndDate.Date);

            var currentUser = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync
                (x => x.Id == request.EmployeeId, null, null,
                "EmployeeLeaveGroup," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType.HRYear," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail," +
                "EmployeeLeaveGroup.EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail.EmployeeLeaveType"
                );

            var employeeLeaves = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetAsync
                (x => x.StatusId != 4 && x.EmployeeId == request.EmployeeId && x.IsActive && !x.IsDelete && x.CreatedDate.Value.Date >= hrYear.StartDate.Date && x.CreatedDate.Value.Date <= hrYear.EndDate.Date, null, null,
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

            // 🚫 Employee not confirmed → no leave allowed
            if (!currentUser.DateOfConfirmation.HasValue || today < currentUser.DateOfConfirmation.Value.Date)
            {
                return currentUser.EmployeeLeaveGroup.EmployeeGroupLeaveType
                    .Where(x => today >= x.HRYear.StartDate.Date && today <= x.HRYear.EndDate.Date)
                    .SelectMany(x => x.EmployeeGroupLeaveTypeDetail)
                    .Select(detail => new LeaveBalanceDto
                    {
                        Id = detail.Id,
                        LeaveType = detail.EmployeeLeaveType.Name,
                        Allotted = 0,
                        Used = 0,
                        Balance = 0
                    })
                    .ToList();
            }

            var leaveBalances = leaveDetails.Select(detail =>
            {

                // 🚫 Not confirmed → no leave allowed
                if (!currentUser.DateOfConfirmation.HasValue ||
                    today < currentUser.DateOfConfirmation.Value.Date)
                {
                    return new LeaveBalanceDto
                    {
                        Id = detail.Id,
                        LeaveType = detail.EmployeeLeaveType.Name,
                        Allotted = 0,
                        Used = 0,
                        Balance = 0
                    };
                }
                else
                {


                    // Get the used leaves for this type
                    double used = usedLeavesByType.FirstOrDefault(x => x.LeaveTypeId == detail.EmployeeLeaveTypeId)?.UsedDays ?? 0;

                    // Prorate the leave based on DateOfConfirmation and months worked
                    double proratedNoOfLeaves = detail.NoOfLeaves;

                    // If the employee's DateOfConfirmation is within the HR Year range, we need to prorate the leave
                    if (currentUser.DateOfConfirmation >= hrYear.StartDate.Date && currentUser.DateOfConfirmation <= hrYear.EndDate.Date && detail.EmployeeLeaveType.Name == "Annual Leave")
                    {
                        // Get the total months in the HR year
                        var totalMonthsInYear = 12;

                        var monthsWorked =
                            (hrYear.EndDate.Year - currentUser.DateOfConfirmation.Value.Year) * 12
                            + hrYear.EndDate.Month
                            - currentUser.DateOfConfirmation.Value.Month
                            + 1;

                        proratedNoOfLeaves = (double)Math.Round(
                           (Convert.ToDecimal(detail.NoOfLeaves) / totalMonthsInYear) * monthsWorked,
                           2
                       );
                    }

                    // Return the leave balance DTO with prorated leave and used leave
                    return new LeaveBalanceDto
                    {
                        Id = detail.Id,
                        LeaveType = detail.EmployeeLeaveType.Name,
                        Allotted = (decimal)proratedNoOfLeaves,
                        Used = used,
                        Balance = proratedNoOfLeaves - used  // Calculate balance using prorated leaves
                    };
                }
            }).ToList();

            return leaveBalances;
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.UserAttendance.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.userAttendance.Handler
{
    public class SaveUserAtendanceHandler : IRequestHandler<SaveUserAttendanceCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveUserAtendanceHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveUserAttendanceCommand, long>.Handle(SaveUserAttendanceCommand request, CancellationToken cancellationToken)
        {
            var userAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id, null, null, "EmployeeShift");
            var employee = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync(x => x.Id == request.UserId);
            var employeeshift = await unitOfWork.Repository<Entities.Models.EmployeeShift>().GetFirstAsNoTrackingAsync(x => x.Id == employee.EmployeeShiftId);

            decimal? _OvertimeHours = 0;
            decimal? _WorkingHours = GetWorkingHours(request.TimeIn, request.TimeOut);
            DateTime start = userAttendance == null ? Convert.ToDateTime(employeeshift.FromTime) : Convert.ToDateTime(userAttendance.EmployeeShift.FromTime);
            DateTime end = userAttendance == null ? Convert.ToDateTime(employeeshift.ToTime) : Convert.ToDateTime(userAttendance.EmployeeShift.ToTime);
            var _TotalWorkingHours = CalculateTotalWorkingHours(start, end);

            if (_WorkingHours > _TotalWorkingHours)
                _OvertimeHours = _WorkingHours - _TotalWorkingHours;
            else
                _OvertimeHours = 0;

            if (userAttendance == null)
            {
                var _userAttendance = mapper.Map<Entities.Models.UserAttendance>(request);
                _userAttendance.CreatedById = sessionProvider.Session.LoggedInUserId;
                _userAttendance.CreatedDate = DateTime.Now;
                _userAttendance.ManualById = sessionProvider.Session.LoggedInUserId;
                _userAttendance.EmployeeShiftId = employee.EmployeeShiftId;
                _userAttendance.WorkingHours = _WorkingHours;
                _userAttendance.OverTimeHours = _OvertimeHours;
                _userAttendance.IsPresent = true;
                unitOfWork.Repository<Entities.Models.UserAttendance>().Add(_userAttendance);
                SaveChanges();
            }
            else
            {
                var _userAttendance = mapper.Map<Entities.Models.UserAttendance>(userAttendance);
                _userAttendance.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _userAttendance.ModifiedDate = DateTime.Now;
                _userAttendance.ManualById = sessionProvider.Session.LoggedInUserId;
                _userAttendance.WorkingHours = _WorkingHours;
                _userAttendance.OverTimeHours = _OvertimeHours;
                _userAttendance.IsPresent = true;
                _userAttendance.TimeIn = request.TimeIn;
                _userAttendance.TimeOut = request.TimeOut;
                _userAttendance.IsManualIn = request.IsManualIn;
                _userAttendance.IsManualOut = request.IsManualOut;
                unitOfWork.Repository<Entities.Models.UserAttendance>().Update(_userAttendance);
                SaveChanges();
            }
            return 200;
        }

        private decimal CalculateTotalWorkingHours(DateTime start, DateTime end)
        {
            if (start > end)
            {
                end = end.AddDays(1); // Adjust for overnight shift
            }

            var duration = end - start;
            return Math.Round((decimal)duration.TotalHours, 2);
        }

        private decimal GetWorkingHours(DateTime inTime, DateTime outTime)
        {
            try
            {
                if (inTime == DateTime.MinValue || outTime == DateTime.MinValue)
                    return 0;

                // Handle overnight shifts
                DateTime adjustedOutTime = outTime;
                if (outTime < inTime)
                {
                    adjustedOutTime = outTime.AddDays(1);
                }

                TimeSpan duration = adjustedOutTime - inTime;

                if (duration.TotalMinutes <= 0)
                    return 0;

                return Math.Round((decimal)duration.TotalHours, 2);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
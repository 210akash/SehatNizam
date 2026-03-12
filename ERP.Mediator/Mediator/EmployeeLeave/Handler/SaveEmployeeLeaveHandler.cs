using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeave.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class SaveEmployeeLeaveHandler : IRequestHandler<SaveEmployeeLeaveCommand, string>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeLeaveHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<string> IRequestHandler<SaveEmployeeLeaveCommand, string>.Handle(SaveEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            // Retrieve all attendance records within the leave date range
            var userAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>()
                .FindAllAsNoTrackingAsync(y => y.UserId == sessionProvider.Session.LoggedInUserId
                                              && y.AttendanceDate >= request.StartDate
                                              && y.AttendanceDate <= request.EndDate);

            // Identify conflicting dates where the employee was present
            var conflictingDates = userAttendance
                .Where(x => x.AttendanceDate.Date >= request.StartDate.Date
                            && x.AttendanceDate.Date <= request.EndDate.Date)
                .Select(x => x.AttendanceDate.Date)
                .ToList();

            if (conflictingDates.Any() && request.IsFirstHalfDay == false)
            {
                var conflictingDatesString = string.Join(", ", conflictingDates.Select(d => d.ToString("yyyy-MM-dd")));
                return $"Conflict : Employee is present on the following date(s): {conflictingDatesString}.";
            }
            else
            {
                if (request.IsFirstHalfDay == true)
                {
                    var isPresentOnStartDate = conflictingDates.Contains(request.StartDate.Date);
                    if (isPresentOnStartDate)
                    {
                        return $"Conflict : Employee is present on the start date {request.StartDate.ToString("yyyy-MM-dd")}. Leave cannot be applied for this date as a first half day.";
                    }
                }
            }

            var _employeeLeave = mapper.Map<Entities.Models.EmployeeLeave>(request);
            _employeeLeave.CreatedById = sessionProvider.Session.LoggedInUserId;
            _employeeLeave.EmployeeId = sessionProvider.Session.LoggedInUserId;
            _employeeLeave.StatusId = 1;
            _employeeLeave.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.EmployeeLeave>().Add(_employeeLeave);
            SaveChanges();
            return "Success"; // Return success if leave is successfully saved
        }
    }
}
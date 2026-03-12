using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.UserAttendance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetUserAttendanceByUserHandler : IRequestHandler<GetUserAttendanceByUserQuery, List<GetUserAttendance>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetUserAttendanceByUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetUserAttendance>> Handle(GetUserAttendanceByUserQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch existing attendances
            Expression<Func<Entities.Models.UserAttendance, bool>> predicate = x =>
                x.IsActive &&
                x.AttendanceDate >= request.FDate &&
                x.AttendanceDate <= request.TDate.AddDays(1).AddSeconds(-1) &&
                x.UserId == request.UserId;

            var orderByDesc = new Func<IQueryable<Entities.Models.UserAttendance>, IOrderedQueryable<Entities.Models.UserAttendance>>(
                q => q.OrderByDescending(x => x.AttendanceDate)
            );

            var userAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>()
                .GetAsync(predicate, null, orderByDesc, "EmployeeShift,InDevice,OutDevice", null, null);

            var attendanceByDate = userAttendance
                .GroupBy(x => x.AttendanceDate.Date)
                .ToDictionary(g => g.Key, g => g.First());

            // 2. Fetch employee working days
            var workingDays = await unitOfWork.Repository<EmployeeWorkingDays>()
                .GetFirstAsync(x => x.EmployeeId == request.UserId && x.IsActive && !x.IsDelete);

            // 3. Fetch all holidays in the range
            var holidays = await unitOfWork.Repository<Entities.Models.Holiday>()
                .GetAsync(x => x.Date >= request.FDate && x.Date <= request.TDate && x.IsActive && !x.IsDelete);

            var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet(); // for fast lookup

            // 4. Loop over date range and build final list
            var result = new List<GetUserAttendance>();
            for (var date = request.FDate.Date; date <= request.TDate.Date; date = date.AddDays(1))
            {
                if (attendanceByDate.TryGetValue(date, out var attendance))
                {
                    // Attendance exists
                    var dto = mapper.Map<GetUserAttendance>(attendance);
                    dto.Status = "Present"; // Add status if not present in DTO
                    result.Add(dto);
                }
                else if (holidayDates.Contains(date))
                {
                    // It's a holiday
                    result.Add(new GetUserAttendance
                    {
                        AttendanceDate = date,
                        Status = "Public Holiday"  
                    });
                }
                else
                {
                    // No attendance, check if it's a working day
                    string dayName = date.DayOfWeek.ToString(); // e.g., "Monday"

                    // Use reflection or a switch to map day name to property
                    bool isWorkingDay = dayName switch
                    {
                        "Monday" => workingDays.Monday,
                        "Tuesday" => workingDays.Tuesday,
                        "Wednesday" => workingDays.Wednesday,
                        "Thursday" => workingDays.Thursday,
                        "Friday" => workingDays.Friday,
                        "Saturday" => workingDays.Saturday,
                        "Sunday" => workingDays.Sunday,
                        _ => false
                    };

                    result.Add(new GetUserAttendance
                    {
                        AttendanceDate = date,
                        Status = isWorkingDay ? "Absent" : "OFF"
                    });
                }
            }

            // Optional: Order by date descending if needed
            return result.OrderByDescending(x => x.AttendanceDate).ToList();
        }

        public async Task<List<GetUserAttendance>> Handle1(GetUserAttendanceByUserQuery request, CancellationToken cancellationToken)
        {
            // Filter predicate
            Expression<Func<Entities.Models.UserAttendance, bool>> predicate = x =>
                x.IsActive == true &&
                x.AttendanceDate >= request.FDate &&
                x.AttendanceDate <= request.TDate.AddDays(1).AddSeconds(-1) &&
                x.UserId == request.UserId;

            // ✅ Correct orderByDec definition
            Func<IQueryable<Entities.Models.UserAttendance>, IOrderedQueryable<Entities.Models.UserAttendance>> orderByDesc =
                q => q.OrderByDescending(x => x.AttendanceDate);

            // Call repository
            var userAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>()
                .GetAsync(predicate, null, orderByDesc, "EmployeeShift,InDevice,OutDevice,EmployeeWorkingDays", null, null);

            // Map to DTO
            var mappedAttendance = mapper.Map<List<GetUserAttendance>>(userAttendance);

            return mappedAttendance;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using ERP.Services.Interfaces;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Error.Command;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.SqlServer.Management.XEvent;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceAttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;
        private readonly IMediator mediator;
        private readonly SessionProvider sessionProvider;

        public DeviceAttendanceController(IAttendanceService attendanceService, IMediator mediator, SessionProvider sessionProvider)
        {
            this.attendanceService = attendanceService;
            this.mediator = mediator;
            this.sessionProvider = sessionProvider;
        }

        [HttpGet("SyncAttendanceByDate")]
        public async Task<ActionResult<List<Tuple<long, string>>>> SyncAttendanceByDate(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return await attendanceService.SyncAttendanceByDate(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return StatusCode(500, "An error occurred while fetching logs.");
            }
        }

        [HttpGet("SyncAttendanceByEmployee")]
        public async Task<ActionResult<List<Tuple<long, string>>>> SyncAttendanceByEmployee(string EmployeeId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return await attendanceService.SyncAttendanceByEmployeeAsync(EmployeeId, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                // If an error occurs, log it to your error system
                try
                {
                    var user = sessionProvider.Session.LoggedInUserId;  // You can adjust this if you need a specific way to get the user id

                    var model = new AddErrorCommand()
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        UserId = user != null ? new Guid(user.ToString()) : (Guid?)null
                    };

                    var sensorHistory = await mediator.Send(model);
                }
                catch (Exception logEx)
                {
                    // Handle any error that might occur during logging itself
                    Console.WriteLine($"Error while logging the error: {logEx.Message}");
                }

                // Optionally rethrow the original exception
                // Optionally log ex
                return StatusCode(500, "An error occurred while fetching logs.");
            }
        }
    }
}

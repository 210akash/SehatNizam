using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ERP.Mediator.Mediator.Dashboard.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator mediator;

        public DashboardController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetHRDashboardData")]
        public async Task<ActionResult<GetHRDashboardData>> GetHRDashboardData()
        {
            try
            {
                return await this.mediator.Send(new GetHRDashboardQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTodayAttendance")]
        public async Task<ActionResult<GetTodayAttendance>> GetTodayAttendance()
        {
            try
            {
                return await this.mediator.Send(new GetTodayAttendanceQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTodayInterviews")]
        public async Task<ActionResult<List<GetInterview>>> GetTodayInterviews()
        {
            try
            {
                return await this.mediator.Send(new GetTodayInterviewsQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

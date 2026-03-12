using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UserAttendance.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.UserAttendance.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserAttendanceController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserAttendanceController(IMediator mediator)
        {   
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetUserAttendanceById")]
        public async Task<ActionResult<GetUserAttendance>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetUserAttendanceByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllUserAttendance")]
        public async Task<ActionResult<Tuple<IEnumerable<GetUserAttendance>, long>>> GetAll(GetAllUserAttendanceQuery getAllUserAttendanceQuery)
        {
            try
            {
                return await this.mediator.Send(getAllUserAttendanceQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveUserAttendance")]
        public async Task<IActionResult> Save(SaveUserAttendanceCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    var result = await this.mediator.Send(command);
                    if (result == 200)
                    {
                        return this.Result(ResponseStatus.OK, "Employee Attendance Saved!", null);
                    }
                    else
                    {
                        return this.Result(ResponseStatus.Error, "There is some error!", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserAttendanceByName")]
        public async Task<ActionResult<List<GetUserAttendance>>> GetUserAttendanceByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetUserAttendanceByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteUserAttendance")]
        public async Task<ActionResult<long>> DeleteUserAttendance(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteUserAttendanceQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "UserAttendance Not Exist!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting UserAttendance!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetZonesByUserInAttendance")]
        public async Task<ActionResult<List<GetZone>>> GetZonesByUserInAttendance(Guid UserId)
        {
            try
            {
                return await this.mediator.Send(new GetZonesByUserInAttendanceQuery(UserId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserAttendanceByUser")]
        public async Task<ActionResult<List<GetUserAttendance>>> GetUserAttendanceByUser(GetUserAttendanceByUserQuery getUserAttendanceByUserQuery)
        {
            try
            {
                return await this.mediator.Send(getUserAttendanceByUserQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

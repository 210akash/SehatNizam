using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AppointmentType.Query;
using ERP.Mediator.Mediator.AppointmentType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public AppointmentTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAppointmentTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointmentType>, long>>> GetAll(GetAllAppointmentTypeQuery getAllAppointmentTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAppointmentTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAppointmentType")]
        public async Task<IActionResult> Save(SaveAppointmentTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "AppointmentType Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);
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

        [HttpDelete]
        [Route("DeleteAppointmentType")]
        public async Task<ActionResult<bool>> DeleteAppointmentType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAppointmentTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAppointmentTypeCode")]
        public async Task<ActionResult<string>> GetAppointmentTypeCode()
        {
            try
            {
                string code =  await mediator.Send(new GetAppointmentTypeCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator mediator;

        public AppointmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAppointments")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointment>, long>>> GetAllAppointments(GetAllAppointmentQuery command)
        {
            try
            {
                return await this.mediator.Send(command);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllAppointmentByDoctor")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointment>, long>>> GetAllAppointmentByDoctor(GetAllAppointmentByDoctorQuery command)
        {
            try
            {
                return await this.mediator.Send(command);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllAppointmentStatus")]
        public async Task<ActionResult<List<GetAppointmentStatus>>> GetAllAppointmentStatus()
        {
            try
            {
                return await this.mediator.Send(new GetAllAppointmentStatusQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAppointment")]
        public async Task<IActionResult> Save(SaveAppointmentCommand command)
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
                        return this.Result(ResponseStatus.OK, "Appointment Saved!", null);
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
    }
}

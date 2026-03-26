using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.Mediator.Mediator.Appointment.Command;
using Microsoft.AspNetCore.Authorization;

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

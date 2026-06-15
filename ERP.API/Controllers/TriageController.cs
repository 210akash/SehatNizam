using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.Mediator.Mediator.Triage.Command;
using Microsoft.AspNetCore.Authorization;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Triage.Query;
using System.Collections.Generic;
using ERP.Mediator.Mediator.Appointment.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TriageController : ControllerBase
    {
        private readonly IMediator mediator;

        public TriageController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllTriage")]
        public async Task<ActionResult<Tuple<IEnumerable<GetTriage>, long>>> GetAll(GetAllTriageQuery getAllTriageQuery)
        {
            try
            {
                return await this.mediator.Send(getAllTriageQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveTriage")]
        public async Task<IActionResult> Save(SaveTriageCommand command)
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
                        var appoinment = await this.mediator.Send(new GetAppoinmentByIdQuery(command.AppointmentId));
                        return this.Result(ResponseStatus.OK, appoinment, "Triage Saved!");
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

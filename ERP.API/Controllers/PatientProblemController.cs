using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.Mediator.Mediator.PatientProblem.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientProblemController : ControllerBase
    {
        private readonly IMediator mediator;

        public PatientProblemController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("SavePatientProblem")]
        public async Task<IActionResult> Save(SavePatientProblemCommand command)
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
                        return this.Result(ResponseStatus.OK, "Patient Problem Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Already Exists!", null);
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

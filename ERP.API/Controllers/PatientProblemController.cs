using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.Mediator.Mediator.City.Query;
using ERP.Mediator.Mediator.PatientProblem.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
                    return this.Result(ResponseStatus.Error, null,
                        this.GetModelValidationErrors(this.ModelState));
                }

                var result = await this.mediator.Send(command);

                if (result > 0)
                {
                    return this.Result(ResponseStatus.OK,result,
                        "Patient Problem Saved!");
                }
                else if (result == -404)
                {
                    return this.Result(ResponseStatus.Error,
                        null,
                        "Patient Problem not found!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Conflict,
                        "Already Exists!",
                        null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error,
                        "There is some error!",
                        null);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeletePatientProblem")]
        public async Task<ActionResult<bool>> DeletePatientProblem(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePatientProblemCommand(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

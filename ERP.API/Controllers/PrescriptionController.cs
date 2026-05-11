using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.Mediator.Mediator.PatientProblem.Command;
using ERP.Mediator.Mediator.Prescription.Command;
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
    public class PrescriptionController : ControllerBase
    {
        private readonly IMediator mediator;

        public PrescriptionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("SavePrescription")]
        public async Task<IActionResult> Save(SavePrescriptionCommand command)
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
                    return this.Result(ResponseStatus.OK, result,
                        "Prescription Saved!"
                        );
                }
                else if (result == -404)
                {
                    return this.Result(ResponseStatus.RecordNotFound,
                        null,
                        "Prescription not found!");
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
        [Route("DeletePrescription")]
        public async Task<ActionResult<bool>> DeletePrescription(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePrescriptionCommand(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

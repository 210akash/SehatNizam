using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.AdmissionServices.Command;
using ERP.Mediator.Mediator.IPD.AdmissionServices.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionServiceController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdmissionServiceController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
        [Route("GetAllAdmissionServices")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointmentPayment>, long>>> GetAll(GetAllAdmissionServicesQuery getAllAdmissionServiceQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAdmissionServiceQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAdmissionService")]
        public async Task<IActionResult> Save(SaveAdmissionServicesCommand command)
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
                        return this.Result(ResponseStatus.OK, "AdmissionService Saved!", null);
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
        [Route("DeleteAdmissionService")]
        public async Task<ActionResult<bool>> DeleteAdmissionService(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAdmissionServicesQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

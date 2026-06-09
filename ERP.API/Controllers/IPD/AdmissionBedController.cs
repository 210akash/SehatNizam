using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.AdmissionBed.Command;
using ERP.Mediator.Mediator.IPD.AdmissionBed.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionBedController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdmissionBedController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
        [Route("GetAllAdmissionBeds")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAdmissionBed>, long>>> GetAll(GetAllAdmissionBedQuery getAllAdmissionBedQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAdmissionBedQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAdmissionBed")]
        public async Task<IActionResult> Save(SaveAdmissionBedCommand command)
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
                        return this.Result(ResponseStatus.OK, "AdmissionBed Saved!", null);
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
        [Route("DeleteAdmissionBed")]
        public async Task<ActionResult<bool>> DeleteAdmissionBed(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAdmissionBedQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

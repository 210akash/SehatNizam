using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ServiceType.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.ServiceType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public ServiceTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllServiceType")]
        public async Task<ActionResult<Tuple<IEnumerable<GetServiceType>, long>>> GetAll(GetAllServiceTypesQuery getAllServiceTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllServiceTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveServiceType")]
        public async Task<IActionResult> Save(SaveServiceTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "ServiceType Saved!", null);
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

        [HttpGet]
        [Route("DeleteServiceType")]
        public async Task<ActionResult<long>> DeleteServiceType(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteServiceTypeCommand(id));
                if (result == true)
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
    }
}

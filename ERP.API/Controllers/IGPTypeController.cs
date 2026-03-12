using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.IGPType.Query;
using ERP.Mediator.Mediator.IGPType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IGPTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public IGPTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }
  
        [HttpPost]
        [Route("GetAllIGPType")]
        public async Task<ActionResult<Tuple<IEnumerable<GetIGPType>, long>>> GetAll(GetAllIGPTypeQuery getAllIGPTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllIGPTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveIGPType")]
        public async Task<IActionResult> Save(SaveIGPTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "IGPType Saved!", null);
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
        [Route("DeleteIGPType")]
        public async Task<ActionResult<bool>> DeleteIGPType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteIGPTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

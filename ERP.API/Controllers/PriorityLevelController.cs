using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.PriorityLevel.Query;
using ERP.Mediator.Mediator.PriorityLevel.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PriorityLevelController : ControllerBase
    {
        private readonly IMediator mediator;

        public PriorityLevelController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllPriorityLevels")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPriorityLevel>, long>>> GetAll(GetAllPriorityLevelQuery getAllPriorityLevelQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPriorityLevelQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePriorityLevel")]
        public async Task<IActionResult> Save(SavePriorityLevelCommand command)
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
                        return this.Result(ResponseStatus.OK, "PriorityLevel Saved!", null);
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
        [Route("DeletePriorityLevel")]
        public async Task<ActionResult<bool>> DeletePriorityLevel(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePriorityLevelQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

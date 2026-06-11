using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.ComponentType.Command;
using ERP.Mediator.Mediator.BloodBank.ComponentType.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers.BloodBank
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BloodComponentTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public BloodComponentTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllBloodComponentTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetBloodComponentType>, long>>> GetAll(GetAllBloodComponentTypeQuery query)
        {
            try
            {
                return await mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveBloodComponentType")]
        public async Task<IActionResult> Save(SaveBloodComponentTypeCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Blood Component Type Saved!", null);
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);
                }

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteBloodComponentType")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                return await mediator.Send(new DeleteBloodComponentTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.BloodUnit.Command;
using ERP.Mediator.Mediator.BloodBank.BloodUnit.Query;
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
    public class BloodUnitController : ControllerBase
    {
        private readonly IMediator mediator;

        public BloodUnitController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllBloodUnits")]
        public async Task<ActionResult<Tuple<IEnumerable<GetBloodUnit>, long>>> GetAll(GetAllBloodUnitQuery query)
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

        [HttpGet]
        [Route("GetBloodUnitById")]
        public async Task<ActionResult<GetBloodUnit>> GetById(long id)
        {
            try
            {
                return await mediator.Send(new GetBloodUnitByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveBloodUnit")]
        public async Task<IActionResult> Save(SaveBloodUnitCommand command)
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
                    return this.Result(ResponseStatus.OK, "Blood Unit Saved!", null);
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, "Blood Unit Not Found!", null);
                }

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteBloodUnit")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                return await mediator.Send(new DeleteBloodUnitQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

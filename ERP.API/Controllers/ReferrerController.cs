using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Referrer.Query;
using ERP.Mediator.Mediator.Referrer.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReferrerController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReferrerController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllReferrer")]
        public async Task<ActionResult<Tuple<IEnumerable<GetReferrer>, long>>> GetAll(GetAllReferrerQuery getAllReferrerQuery)
        {
            try
            {
                return await this.mediator.Send(getAllReferrerQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveReferrer")]
        public async Task<IActionResult> Save(SaveReferrerCommand command)
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
                        return this.Result(ResponseStatus.OK, "Referrer Saved!", null);
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
        [Route("GetReferrerByName")]
        public async Task<ActionResult<List<GetReferrer>>> GetReferrerByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetReferrerByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteReferrer")]
        public async Task<ActionResult<bool>> DeleteReferrer(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteReferrerQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

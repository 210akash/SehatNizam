using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.RejectReason.Query;
using ERP.Mediator.Mediator.RejectReason.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RejectReasonController : ControllerBase
    {
        private readonly IMediator mediator;

        public RejectReasonController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
        [Route("GetAllRejectReasons")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRejectReason>, long>>> GetAll(GetAllRejectReasonQuery getAllRejectReasonQuery)
        {
            try
            {
                return await this.mediator.Send(getAllRejectReasonQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveRejectReason")]
        public async Task<IActionResult> Save(SaveRejectReasonCommand command)
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
                        return this.Result(ResponseStatus.OK, "RejectReason Saved!", null);
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
        [Route("DeleteRejectReason")]
        public async Task<ActionResult<bool>> DeleteRejectReason(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteRejectReasonQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

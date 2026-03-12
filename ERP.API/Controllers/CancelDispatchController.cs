using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dispatch.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.Mediator.Mediator.CancelDispatch.Query;
using ERP.Mediator.Mediator.CancelDispatch.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CancelDispatchController : ControllerBase
    {
        private readonly IMediator mediator;

        public CancelDispatchController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("getPendingCancelOrder")]
        public async Task<ActionResult<List<GetOrder>>> getPendingCancelOrder(long CancelDispatchId)
        {
            try
            {
                return await mediator.Send(new GetPendingCancelOrderQuery(CancelDispatchId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingCanelOrderItems")]
        public async Task<ActionResult<List<GetOrderItems>>> GetPendingCanelOrderItems(long OrderId, long CancelDispatchId)
        {
            try
            {
                return await mediator.Send(new GetPendingCancelOrderItemsQuery(OrderId, CancelDispatchId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveCancelDispatch")]
        public async Task<IActionResult> Save(SaveCancelDispatchCommand command)
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
                        return this.Result(ResponseStatus.OK, "Cancel Dispatch Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Cancel of This order Already Exists!", null);
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

        [HttpPost]
        [Route("GetAllCancelDispatches")]
        public async Task<ActionResult<Tuple<IEnumerable<GetCancelDispatch>, long>>> GetAllCancelDispatches(GetAllCancelDispatchQuery getAllCancelDispatchQuery)
        {
            try
            {
                return await this.mediator.Send(getAllCancelDispatchQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCancelDispatchCount")]
        public async Task<ActionResult<Tuple<long, long, long, long, long>>> GetDispatchCount(GetCancelDispatchCountQuery getCancelDispatchCountQuery)
        {
            try
            {
                return await this.mediator.Send(getCancelDispatchCountQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ProcessCancelDispatch")]
        public async Task<ActionResult<bool>> ProcessCancelDispatch(ProcessCancelDispatchCommand processCancelDispatchCommand)
        {
            try
            {
                return await this.mediator.Send(processCancelDispatchCommand);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCancelDispatchCode")]
        public async Task<ActionResult<string>> GetCancelDispatchCode()
        {
            try
            {
                string code = await mediator.Send(new GetCancelDispatchCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

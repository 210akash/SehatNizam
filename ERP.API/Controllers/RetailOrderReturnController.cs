using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Mediator.Mediator.RetailOrderReturn.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RetailOrderReturnController : ControllerBase
    {
        private readonly IMediator mediator;

        public RetailOrderReturnController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllRetailOrderReturns")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRetailOrderReturn>, long>>> GetAll(GetAllRetailOrderReturnQuery getAllRetailOrderReturnQuery)
        {
            try
            {
                return await this.mediator.Send(getAllRetailOrderReturnQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveRetailOrderReturn")]
        public async Task<IActionResult> Save(SaveRetailOrderReturnCommand command)
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
                        return this.Result(ResponseStatus.OK, "RetailOrderReturn Saved!", null);
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
        [Route("DeleteRetailOrderReturn")]
        public async Task<ActionResult<bool>> DeleteRetailOrderReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteRetailOrderReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetRetailOrderReturnCode")]
        public async Task<ActionResult<string>> GetRetailOrderReturnCode()
        {
            try
            {
                string code = await mediator.Send(new GetRetailOrderReturnCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessRetailOrderReturn")]
        public async Task<ActionResult<bool>> ProcessRetailOrderReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessRetailOrderReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
          
        [HttpPost]
        [Route("GetRetailOrderReturnCount")]
        public async Task<ActionResult<Tuple<long, long>>> GetRetailOrderReturnCount(GetRetailOrderReturnCountQuery getLeadsCountByUserProjectQuery)
        {
            try
            {
                return await this.mediator.Send(getLeadsCountByUserProjectQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPendingRetailOrder")]
        public async Task<ActionResult<List<GetRetailOrder>>> GetPendingRetailOrder(GetPendingRetailOrderQuery getPendingRetailOrderQuery)
        {
            try
            {
                return await mediator.Send(getPendingRetailOrderQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingRetailOrderItems")]
        public async Task<ActionResult<List<GetRetailOrderItems>>> GetPendingRetailOrderItems(long RetailOrderId, long RetailOrderReturnId)
        {
            try
            {
                return await mediator.Send(new GetPendingRetailOrderItemsQuery(RetailOrderId, RetailOrderReturnId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

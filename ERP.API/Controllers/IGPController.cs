using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Mediator.Mediator.IGP.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IGPController : ControllerBase
    {
        private readonly IMediator mediator;

        public IGPController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetIGPById")]
        public async Task<ActionResult<GetIGP>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetIGPByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllIGPs")]
        public async Task<ActionResult<Tuple<IEnumerable<GetIGP>, long>>> GetAll(GetAllIGPQuery getAllIGPQuery)
        {
            try
            {
                return await this.mediator.Send(getAllIGPQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveIGP")]
        public async Task<IActionResult> Save(SaveIGPCommand command)
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
                        return this.Result(ResponseStatus.OK, "IGP Saved!", null);
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
        [Route("DeleteIGP")]
        public async Task<ActionResult<bool>> DeleteIGP(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteIGPQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetIGPCode")]
        public async Task<ActionResult<string>> GetIGPCode()
        {
            try
            {
                string code = await mediator.Send(new GetIGPCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessIGP")]
        public async Task<ActionResult<bool>> ProcessIGP(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessIGPQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetIGPCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetIGPCount(GetIGPCountQuery getLeadsCountByUserProjectQuery)
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

        [HttpGet]
        [Route("GetPendingPurchaseOrders")]
        public async Task<ActionResult<List<GetDropDown>>> GetPendingPurchaseOrders(long PurchaseOrderId)
        {
            try
            {
                return await mediator.Send(new GetPendingPurchaseOrdersQuery(PurchaseOrderId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingPOItems")]
        public async Task<ActionResult<List<GetPurchaseOrderDetail>>> GetPendingPOItems(long PurchaseOrderId)
        {
            try
            {
                return await mediator.Send(new GetPendingPOItemsQuery(PurchaseOrderId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

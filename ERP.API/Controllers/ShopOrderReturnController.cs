using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Mediator.Mediator.ShopOrderReturn.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShopOrderReturnController : ControllerBase
    {
        private readonly IMediator mediator;

        public ShopOrderReturnController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllShopOrderReturns")]
        public async Task<ActionResult<Tuple<IEnumerable<GetShopOrderReturn>, long>>> GetAll(GetAllShopOrderReturnQuery getAllShopOrderReturnQuery)
        {
            try
            {
                return await this.mediator.Send(getAllShopOrderReturnQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveShopOrderReturn")]
        public async Task<IActionResult> Save(SaveShopOrderReturnCommand command)
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
                        return this.Result(ResponseStatus.OK, "ShopOrderReturn Saved!", null);
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
        [Route("DeleteShopOrderReturn")]
        public async Task<ActionResult<bool>> DeleteShopOrderReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteShopOrderReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShopOrderReturnCode")]
        public async Task<ActionResult<string>> GetShopOrderReturnCode()
        {
            try
            {
                string code = await mediator.Send(new GetShopOrderReturnCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessShopOrderReturn")]
        public async Task<ActionResult<bool>> ProcessShopOrderReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessShopOrderReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
          
        [HttpPost]
        [Route("GetShopOrderReturnCount")]
        public async Task<ActionResult<Tuple<long, long>>> GetShopOrderReturnCount(GetShopOrderReturnCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingShopOrder")]
        public async Task<ActionResult<List<GetShopOrder>>> GetPendingShopOrder(GetPendingShopOrderQuery getPendingShopOrderQuery)
        {
            try
            {
                return await mediator.Send(getPendingShopOrderQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingShopOrderItems")]
        public async Task<ActionResult<List<GetShopOrderItems>>> GetPendingShopOrderItems(long OrderId, long ShopOrderReturnId)
        {
            try
            {
                return await mediator.Send(new GetPendingShopOrderItemsQuery(OrderId, ShopOrderReturnId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

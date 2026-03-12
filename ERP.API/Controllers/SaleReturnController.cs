using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Mediator.Mediator.SaleReturn.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleReturnController : ControllerBase
    {
        private readonly IMediator mediator;

        public SaleReturnController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetSaleReturnById")]
        public async Task<ActionResult<GetSaleReturn>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetSaleReturnByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllSaleReturns")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSaleReturn>, long>>> GetAll(GetAllSaleReturnQuery getAllSaleReturnQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSaleReturnQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSaleReturn")]
        public async Task<IActionResult> Save(SaveSaleReturnCommand command)
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
                        return this.Result(ResponseStatus.OK, "SaleReturn Saved!", null);
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
        [Route("DeleteSaleReturn")]
        public async Task<ActionResult<bool>> DeleteSaleReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteSaleReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSaleReturnCode")]
        public async Task<ActionResult<string>> GetSaleReturnCode()
        {
            try
            {
                string code = await mediator.Send(new GetSaleReturnCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessSaleReturn")]
        public async Task<ActionResult<bool>> ProcessSaleReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessSaleReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveSaleReturn")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveSaleReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveSaleReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSaleReturnCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetSaleReturnCount(GetSaleReturnCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingDC")]
        public async Task<ActionResult<List<GetDispatchOrder>>> GetPendingDC(GetPendingDCQuery getPendingDCQuery)
        {
            try
            {
                return await mediator.Send(getPendingDCQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingDCItems")]
        public async Task<ActionResult<List<GetDispatchDetail>>> GetPendingDCItems(long DispatchOrderId, long SaleReturnId)
        {
            try
            {
                return await mediator.Send(new GetPendingDCItemsQuery(DispatchOrderId, SaleReturnId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

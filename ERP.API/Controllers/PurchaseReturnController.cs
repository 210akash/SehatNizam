using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Mediator.Mediator.PurchaseReturn.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseReturnController : ControllerBase
    {
        private readonly IMediator mediator;

        public PurchaseReturnController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetPurchaseReturnById")]
        public async Task<ActionResult<GetPurchaseReturn>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetPurchaseReturnByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPurchaseReturns")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPurchaseReturn>, long>>> GetAll(GetAllPurchaseReturnQuery getAllPurchaseReturnQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPurchaseReturnQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePurchaseReturn")]
        public async Task<IActionResult> Save(SavePurchaseReturnCommand command)
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
                        return this.Result(ResponseStatus.OK, "PurchaseReturn Saved!", null);
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
        [Route("DeletePurchaseReturn")]
        public async Task<ActionResult<bool>> DeletePurchaseReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePurchaseReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPurchaseReturnCode")]
        public async Task<ActionResult<string>> GetPurchaseReturnCode()
        {
            try
            {
                string code = await mediator.Send(new GetPurchaseReturnCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessPurchaseReturn")]
        public async Task<ActionResult<bool>> ProcessPurchaseReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessPurchaseReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApprovePurchaseReturn")]
        public async Task<ActionResult<Tuple<long, string>>> ApprovePurchaseReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ApprovePurchaseReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPurchaseReturnCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetPurchaseReturnCount(GetPurchaseReturnCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingGRN")]
        public async Task<ActionResult<List<GetGRN>>> GetPendingGRN(GetPendingGRNQuery getPendingGRNQuery)
        {
            try
            {
                return await mediator.Send(getPendingGRNQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingGRNItems")]
        public async Task<ActionResult<List<GetGRNDetail>>> GetPendingGRNItems(long GRNId, long PurchaseReturnId)
        {
            try
            {
                return await mediator.Send(new GetPendingGRNItemsQuery(GRNId, PurchaseReturnId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

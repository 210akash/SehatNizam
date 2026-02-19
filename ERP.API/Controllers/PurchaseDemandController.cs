using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Mediator.Mediator.PurchaseDemand.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseDemandController : ControllerBase
    {
        private readonly IMediator mediator;

        public PurchaseDemandController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetPurchaseDemandById")]
        public async Task<ActionResult<GetPurchaseDemand>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetPurchaseDemandByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPurchaseDemands")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPurchaseDemand>, long>>> GetAll(GetAllPurchaseDemandQuery getAllPurchaseDemandQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPurchaseDemandQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePurchaseDemand")]
        public async Task<IActionResult> Save(SavePurchaseDemandCommand command)
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
                        return this.Result(ResponseStatus.OK, "PurchaseDemand Saved!", null);
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
        [Route("DeletePurchaseDemand")]
        public async Task<ActionResult<bool>> DeletePurchaseDemand(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePurchaseDemandQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPurchaseDemandByCompany")]
        public async Task<ActionResult<List<GetPurchaseDemand>>> GetPurchaseDemandByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetPurchaseDemandByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPurchaseDemandCode")]
        public async Task<ActionResult<string>> GetPurchaseDemandCode()
        {
            try
            {
                string code =  await mediator.Send(new GetPurchaseDemandCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessPurchaseDemand")]
        public async Task<ActionResult<bool>> ProcessPurchaseDemand(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessPurchaseDemandQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApprovePurchaseDemand")]
        public async Task<ActionResult<bool>> ApprovePurchaseDemand(long id)
        {
            try
            {
                return await this.mediator.Send(new ApprovePurchaseDemandQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPurchaseDemandCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetPurchaseDemandCount(GetPurchaseDemandCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingIndentRequest")]
        public async Task<ActionResult<List<GetDropDown>>> GetPendingIndentRequest(long IndentRequestId)
        {
            try
            {
                return await mediator.Send(new GetPendingIndentRequestQuery(IndentRequestId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingIndentItems")]
        public async Task<ActionResult<List<GetIndentRequestDetail>>> GetPendingIndentItems(long IndentRequestId, long PurchaseDemandId)
        {
            try
            {
                return await mediator.Send(new GetPendingIndentItemsQuery(IndentRequestId, PurchaseDemandId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

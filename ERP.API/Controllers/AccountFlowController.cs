using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AccountFlow.Query;
using ERP.Mediator.Mediator.AccountFlow.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountFlowController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountFlowController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAccountFlows")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccountFlow>, long>>> GetAll(GetAllAccountFlowQuery getAllAccountFlowQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountFlowQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccountFlow")]
        public async Task<IActionResult> Save(SaveAccountFlowCommand command)
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
                        return this.Result(ResponseStatus.OK, "AccountFlow Saved!", null);
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
        [Route("DeleteAccountFlow")]
        public async Task<ActionResult<bool>> DeleteAccountFlow(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountFlowQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountFlowByCompany")]
        public async Task<ActionResult<List<GetAccountFlow>>> GetAccountFlowByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetAccountFlowByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountFlowCode")]
        public async Task<ActionResult<string>> GetAccountFlowCode()
        {
            try
            {
                string code =  await mediator.Send(new GetAccountFlowCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

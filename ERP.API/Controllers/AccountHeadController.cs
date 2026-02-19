using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AccountHead.Query;
using ERP.Mediator.Mediator.AccountHead.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountHeadController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountHeadController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAccountHeads")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccountHead>, long>>> GetAll(GetAllAccountHeadQuery getAllAccountHeadQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountHeadQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccountHead")]
        public async Task<IActionResult> Save(SaveAccountHeadCommand command)
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
                        return this.Result(ResponseStatus.OK, "AccountHead Saved!", null);
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
        [Route("DeleteAccountHead")]
        public async Task<ActionResult<bool>> DeleteAccountHead(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountHeadQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountHeadByCompany")]
        public async Task<ActionResult<List<GetAccountHead>>> GetAccountHeadByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetAccountHeadByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountHeadCode")]
        public async Task<ActionResult<string>> GetAccountHeadCode()
        {
            try
            {
                string code =  await mediator.Send(new GetAccountHeadCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

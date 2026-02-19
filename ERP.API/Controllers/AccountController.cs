using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Mediator.Mediator.Account.Command;
using ERP.Mediator.Mediator.Currency.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAccounts")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccount>, long>>> GetAll(GetAllAccountQuery getAllAccountQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccount")]
        public async Task<IActionResult> Save(SaveAccountCommand command)
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
                        return this.Result(ResponseStatus.OK, "Account Saved!", null);
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

        [HttpPost]
        [Route("GetAccountByName")]
        public async Task<ActionResult<List<GetAccount>>> GetAccountByName([FromBody] GetAccountByNameRequest request)
        {
            try
            {
                return await mediator.Send(new GetAccountByNameQuery(request.Name, request.AccountFlow));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        // Create a DTO (Data Transfer Object)
        public class GetAccountByNameRequest
        {
            public string Name { get; set; }
            public List<string> AccountFlow { get; set; }
        }

        [HttpDelete]
        [Route("DeleteAccount")]
        public async Task<ActionResult<bool>> DeleteAccount(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountByCompany")]
        public async Task<ActionResult<List<GetAccount>>> GetAccountByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetAccountByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountCode")]
        public async Task<ActionResult<string>> GetAccountCode(long AccountTypeId, long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetAccountCodeQuery(AccountTypeId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

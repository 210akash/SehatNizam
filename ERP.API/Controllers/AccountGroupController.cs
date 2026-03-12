using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AccountGroup.Query;
using ERP.Mediator.Mediator.AccountGroup.Command;
using ERP.Mediator.Mediator.Currency.Query;
using ERP.Mediator.Mediator.Account.Query;
using static ERP.API.Controllers.AccountController;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountGroupController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountGroupController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAccountGroups")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccountGroup>, long>>> GetAll(GetAllAccountGroupQuery getAllAccountGroupQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountGroupQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccountGroup")]
        public async Task<IActionResult> Save(SaveAccountGroupCommand command)
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
                        return this.Result(ResponseStatus.OK, "AccountGroup Saved!", null);
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
        [Route("DeleteAccountGroup")]
        public async Task<ActionResult<bool>> DeleteAccountGroup(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountGroupQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountGroupCode")]
        public async Task<ActionResult<string>> GetAccountGroupCode(long AccountGroupTypeId, long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetAccountGroupCodeQuery(AccountGroupTypeId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAccountGroupByName")]
        public async Task<ActionResult<List<GetAccountGroup>>> GetAccountGroupByName([FromBody] GetAccountGroupByNameRequest request)
        {
            try
            {
                return await mediator.Send(new GetAccountGroupByNameQuery(request.Name, request.AccountFlow));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        // Create a DTO (Data Transfer Object)
        public class GetAccountGroupByNameRequest
        {
            public string Name { get; set; }
            public List<string> AccountFlow { get; set; }
        }


    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Mediator.Mediator.AccountCategory.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountCategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetAccountCategoryById")]
        public async Task<ActionResult<GetAccountCategory>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetAccountCategoryByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllAccountCategorys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccountCategory>, long>>> GetAll(GetAllAccountCategoryQuery getAllAccountCategoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountCategoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccountCategory")]
        public async Task<IActionResult> Save(SaveAccountCategoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "AccountCategory Saved!", null);
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
        [Route("DeleteAccountCategory")]
        public async Task<ActionResult<bool>> DeleteAccountCategory(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountCategoryQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountCategoryByCompany")]
        public async Task<ActionResult<List<GetAccountCategory>>> GetAccountCategoryByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetAccountCategoryByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountCategoryCode")]
        public async Task<ActionResult<string>> GetAccountCategoryCode()
        {
            try
            {
                string code =  await mediator.Send(new GetAccountCategoryCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

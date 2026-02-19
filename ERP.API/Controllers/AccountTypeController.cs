using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AccountType.Query;
using ERP.Mediator.Mediator.AccountType.Command;
using ERP.Mediator.Mediator.AccountSubCategory.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAccountTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccountType>, long>>> GetAll(GetAllAccountTypeQuery getAllAccountTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccountType")]
        public async Task<IActionResult> Save(SaveAccountTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "AccountType Saved!", null);
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
        [Route("DeleteAccountType")]
        public async Task<ActionResult<bool>> DeleteAccountType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountTypeByCompany")]
        public async Task<ActionResult<List<GetAccountType>>> GetAccountTypeByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetAccountTypeByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountTypeBySubCategory")]
        public async Task<ActionResult<List<GetAccountType>>> GetAccountTypeBySubCategory(long AccountSubCategoryId)
        {
            try
            {
                return await mediator.Send(new GetAccountTypeByAccountSubCategoryQuery(AccountSubCategoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountTypeCode")]
        public async Task<ActionResult<string>> GetAccountTypeCode(long AccountSubCategoryId, long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetAccountTypeCodeQuery(AccountSubCategoryId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

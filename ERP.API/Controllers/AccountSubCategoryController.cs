using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Mediator.Mediator.AccountSubCategory.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountSubCategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountSubCategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetAccountSubCategoryById")]
        public async Task<ActionResult<GetAccountSubCategory>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetAccountSubCategoryByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllAccountSubCategorys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAccountSubCategory>, long>>> GetAll(GetAllAccountSubCategoryQuery getAllAccountSubCategoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAccountSubCategoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAccountSubCategory")]
        public async Task<IActionResult> Save(SaveAccountSubCategoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "AccountSubCategory Saved!", null);
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

        [HttpGet]
        [Route("GetAccountSubCategoryByName")]
        public async Task<ActionResult<List<GetAccountSubCategory>>> GetAccountSubCategoryByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetAccountSubCategoryByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAccountSubCategory")]
        public async Task<ActionResult<bool>> DeleteAccountSubCategory(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAccountSubCategoryQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountSubCategoryByCompany")]
        public async Task<ActionResult<List<GetAccountSubCategory>>> GetAccountSubCategoryByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetAccountSubCategoryByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountSubCategoryByCategory")]
        public async Task<ActionResult<List<GetAccountSubCategory>>> GetAccountSubCategoryByCategory(long AccountCategoryId)
        {
            try
            {
                return await mediator.Send(new GetAccountSubCategoryByCategoryQuery(AccountCategoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAccountSubCategoryCode")]
        public async Task<ActionResult<string>> GetAccountSubCategoryCode(long AccountCategoryId,long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetAccountSubCategoryCodeQuery(AccountCategoryId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

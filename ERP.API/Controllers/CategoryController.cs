using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Category.Query;
using ERP.Mediator.Mediator.Category.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public CategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetCategoryById")]
        public async Task<ActionResult<GetCategory>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetCategoryByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllCategorys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetCategory>, long>>> GetAll(GetAllCategoryQuery getAllCategoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllCategoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveCategory")]
        public async Task<IActionResult> Save(SaveCategoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "Category Saved!", null);
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
        [Route("GetCategoryByName")]
        public async Task<ActionResult<List<GetCategory>>> GetCategoryByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetCategoryByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        public async Task<ActionResult<bool>> DeleteCategory(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteCategoryQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCategoryByCompany")]
        public async Task<ActionResult<List<GetCategory>>> GetCategoryByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetCategoryByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCategoryCode")]
        public async Task<ActionResult<string>> GetCategoryCode()
        {
            try
            {
                string code =  await mediator.Send(new GetCategoryCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

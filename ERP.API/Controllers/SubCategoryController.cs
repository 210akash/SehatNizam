using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Mediator.Mediator.SubCategory.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public SubCategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetSubCategoryById")]
        public async Task<ActionResult<GetSubCategory>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetSubCategoryByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllSubCategorys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSubCategory>, long>>> GetAll(GetAllSubCategoryQuery getAllSubCategoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSubCategoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSubCategory")]
        public async Task<IActionResult> Save(SaveSubCategoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "SubCategory Saved!", null);
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
        [Route("GetSubCategoryByName")]
        public async Task<ActionResult<List<GetSubCategory>>> GetSubCategoryByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetSubCategoryByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteSubCategory")]
        public async Task<ActionResult<bool>> DeleteSubCategory(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteSubCategoryQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSubCategoryByCompany")]
        public async Task<ActionResult<List<GetSubCategory>>> GetSubCategoryByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetSubCategoryByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSubCategoryByCategory")]
        public async Task<ActionResult<List<GetSubCategory>>> GetSubCategoryByCategory(long CategoryId)
        {
            try
            {
                return await mediator.Send(new GetSubCategoryByCategoryQuery(CategoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSubCategoryCode")]
        public async Task<ActionResult<string>> GetSubCategoryCode(long CategoryId,long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetSubCategoryCodeQuery(CategoryId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

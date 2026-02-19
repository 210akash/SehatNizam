using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.ItemType.Query;
using ERP.Mediator.Mediator.ItemType.Command;
using ERP.Mediator.Mediator.SubCategory.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public ItemTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetItemTypeById")]
        public async Task<ActionResult<GetItemType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetItemTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllItemTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetItemType>, long>>> GetAll(GetAllItemTypeQuery getAllItemTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllItemTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveItemType")]
        public async Task<IActionResult> Save(SaveItemTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "ItemType Saved!", null);
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
        [Route("GetItemTypeByName")]
        public async Task<ActionResult<List<GetItemType>>> GetItemTypeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetItemTypeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteItemType")]
        public async Task<ActionResult<bool>> DeleteItemType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteItemTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetItemTypeByCompany")]
        public async Task<ActionResult<List<GetItemType>>> GetItemTypeByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetItemTypeByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetItemTypeCode")]
        public async Task<ActionResult<string>> GetItemTypeCode(long SubCategoryId,long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetItemTypeCodeQuery(SubCategoryId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetItemtypeBySubCategory")]
        public async Task<ActionResult<List<GetItemType>>> GetItemtypeBySubCategory(long SubCategoryId)
        {
            try
            {
                return await mediator.Send(new GetItemtypeBySubCategoryQuery(SubCategoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Mediator.Mediator.Item.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IMediator mediator;

        public ItemController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetItemById")]
        public async Task<ActionResult<GetItem>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetItemByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllItems")]
        public async Task<ActionResult<Tuple<IEnumerable<GetItem>, long>>> GetAll(GetAllItemQuery getAllItemQuery)
        {
            try
            {
                return await this.mediator.Send(getAllItemQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveItem")]
        public async Task<IActionResult> Save(SaveItemCommand command)
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
                        return this.Result(ResponseStatus.OK, "Item Saved!", null);
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
        [Route("GetItemByName")]
        public async Task<ActionResult<List<GetItem>>> GetItemByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetItemByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteItem")]
        public async Task<ActionResult<bool>> DeleteItem(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteItemQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetItemByCompany")]
        public async Task<ActionResult<List<GetItem>>> GetItemByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetItemByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetItemCode")]
        public async Task<ActionResult<string>> GetItemCode(long ItemTypeId, long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetItemCodeQuery(ItemTypeId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

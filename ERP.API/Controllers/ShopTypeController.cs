using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopType.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.ShopType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShopTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public ShopTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetShopTypeById")]
        public async Task<ActionResult<GetShopType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetShopTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllShopType")]
        public async Task<ActionResult<Tuple<IEnumerable<GetShopType>, long>>> GetAll(GetAllShopTypeQuery getAllShopTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllShopTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveShopType")]
        public async Task<IActionResult> Save(SaveShopTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shop Type Saved!", null);
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
        [Route("GetShopTypeByName")]
        public async Task<ActionResult<List<GetShopType>>> GetShopTypeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetShopTypeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteShopType")]
        public async Task<ActionResult<long>> DeleteShopType(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteShopTypeQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Shop Type is used in Shop!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting ShopType!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

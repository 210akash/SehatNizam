using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Shop.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly IMediator mediator;

        public ShopController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetShopById")]
        public async Task<ActionResult<GetShop>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetShopByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllShop")]
        public async Task<ActionResult<Tuple<IEnumerable<GetShop>, long>>> GetAll(GetAllShopQuery getAllShopQuery)
        {
            try
            {
                return await this.mediator.Send(getAllShopQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveShop")]
        public async Task<IActionResult> Save(SaveShopCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shop Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);
                    }
                    else if (result == 412)
                    {
                        return this.Result(ResponseStatus.DuplicatePhoneNo, "Duplicate Phone No!", null);
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
        [Route("GetShopByName")]
        public async Task<ActionResult<List<GetShop>>> GetShopByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetShopByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteShop")]
        public async Task<ActionResult<long>> DeleteShop(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteShopQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Shop is used in Routes!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Shop!");
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

        [HttpGet]
        [Route("GetShopsByTerritoryId")]
        public async Task<ActionResult<List<GetShop>>> GetShopsByTerritoryId(long territoryId)
        {
            try
            {
                return await this.mediator.Send(new GetShopsByTerritoryIdQuery(territoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShopsByRouteId")]
        public async Task<ActionResult<List<GetShop>>> GetShopsByRouteId(long routeId)
        {
            try
            {
                return await this.mediator.Send(new GetShopsByRouteIdQuery(routeId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
        [HttpGet]
        [Route("VerifyShopById")]
        public async Task<ActionResult<long>> VerifyShopById(long id)
        {
            try
            {
                var result = await this.mediator.Send(new VerifyShopByIdQuery(id));

                if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Verify Shop!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Verified!");
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

        [HttpGet]
        [Route("ApproveShop")]
        public async Task<ActionResult<bool>> ApproveShop(long id,string Remarks)
        {
            try
            {
                return await this.mediator.Send(new ApproveShopQuery(id, Remarks));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("RejectShop")]
        public async Task<ActionResult<bool>> RejectShop(long id, string Remarks)
        {
            try
            {
                return await this.mediator.Send(new RejectShopQuery(id, Remarks));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("SearchShopsByTerritoryId")]
        public async Task<ActionResult<List<GetShopBasic>>> SearchShopsByTerritoryId(long territoryId, string param)
        {
            try
            {
                return await this.mediator.Send(new SearchShopsByTerritoryIdQuery(territoryId, param));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Route.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Route.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RouteController : ControllerBase
    {
        private readonly IMediator mediator;

        public RouteController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetRouteById")]
        public async Task<ActionResult<GetRoute>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetRouteByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllRoute")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRoute>, long>>> GetAll(GetAllRouteQuery getAllRouteQuery)
        {
            try
            {
                return await this.mediator.Send(getAllRouteQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveRoute")]
        public async Task<IActionResult> Save(SaveRouteCommand command)
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
                        return this.Result(ResponseStatus.OK, "Route Saved!", null);
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
        [Route("GetRouteByName")]
        public async Task<ActionResult<List<GetRoute>>> GetRouteByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetRouteByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteRoute")]
        public async Task<ActionResult<bool>> DeleteRoute(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteRouteQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Route is used in DSFRoute/RouteShop!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Route!");
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

        [HttpPost]
        [Route("AddShopsRoute")]
        public async Task<IActionResult> AddShopsRoute(AddShopsRouteCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shops Added!", null);
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
        [Route("GetRouteByDSFTerritory")]
        public async Task<ActionResult<List<GetRoute>>> GetRouteByDSFTerritory(string dsfId)
        {
            try
            {
                return await this.mediator.Send(new GetRouteByDSFTerritoryQuery(dsfId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("IsShopOccupied")]
        public async Task<ActionResult<GetShopOccupied>> IsShopOccupied(long shopId, long routeId)
        {
            try
            {
                return await this.mediator.Send(new IsShopOccupiedQuery(shopId, routeId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteRouteShop")]
        public async Task<ActionResult<long>> DeleteRouteShop(long routeShopId)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteRouteShopQuery(routeShopId));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Route can't be deleted!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Route!");
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
        [Route("GetRoutesByDsfId")]
        public async Task<ActionResult<List<GetRoute>>> GetRoutesByDsfId(string DsfId, long TerritoryId)
        {
            try
            {
                return await this.mediator.Send(new GetRoutesByDsfIdQuery(DsfId, TerritoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("AddShopsRouteFrequency")]
        public async Task<IActionResult> AddShopsRouteFrequency(AddShopsRouteFrequencyCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shops Added!", null);
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
        [Route("GetShopRouteFrequencyByTerritoryId")]
        public async Task<ActionResult<List<GetShopRouteFrequency>>> GetShopRouteFrequencyByTerritoryId(long TerritoryId)
        {
            try
            {
                return await this.mediator.Send(new GetShopRouteFrequencyByTerritoryIdQuery(TerritoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

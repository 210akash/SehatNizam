using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Territory.Command;
using ERP.Mediator.Mediator.Zone.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TerritoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public TerritoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetTerritoryById")]
        public async Task<ActionResult<GetTerritory>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetTerritoryByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllTerritory")]
        public async Task<ActionResult<Tuple<IEnumerable<GetTerritory>, long>>> GetAll(GetAllTerritoryQuery getAllTerritoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllTerritoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveTerritory")]
        public async Task<IActionResult> Save(SaveTerritoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "Territory Saved!", null);
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
        [Route("GetTerritoryByName")]
        public async Task<ActionResult<List<GetTerritory>>> GetTerritoryByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetTerritoryByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteTerritory")]
        public async Task<ActionResult<long>> DeleteTerritory(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteTerritoryQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Territory is used in Shops/Dealership!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Territory!");
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
        [Route("GetTerritoryByAreaId")]
        public async Task<ActionResult<List<GetTerritory>>> GetTerritoryByAreaId(long AreaId)
        {
            try
            {
                return await this.mediator.Send(new GetTerritoryByAreaIdQuery(AreaId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDsfByTerritoryId")]
        public async Task<ActionResult<List<GetUsers>>> GetDsfByTerritoryId(long TerritoryId)
        {
            try
            {
                return await this.mediator.Send(new GetDsfByTerritoryIdQuery(TerritoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTerritoryBySaleModel")]
        public async Task<ActionResult<List<GetTerritory>>> GetTerritoryBySaleModel(long AreaId, string saleModel)
        {
            try
            {
                return await this.mediator.Send(new GetTerritoryBySaleModelQuery(AreaId, saleModel));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UserTerritory.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.UserTerritory.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserTerritoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserTerritoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetUserTerritoryById")]
        public async Task<ActionResult<GetUserTerritory>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetUserTerritoryByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllUserTerritory")]
        public async Task<ActionResult<Tuple<IEnumerable<GetUserTerritory>, long>>> GetAll(GetAllUserTerritoryQuery getAllUserTerritoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllUserTerritoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveUserTerritory")]
        public async Task<IActionResult> Save(SaveUserTerritoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "User Territory Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "UserTerritory already Exists!", null);
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
        [Route("GetUserTerritoryByName")]
        public async Task<ActionResult<List<GetUserTerritory>>> GetUserTerritoryByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetUserTerritoryByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteUserTerritory")]
        public async Task<ActionResult<long>> DeleteUserTerritory(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteUserTerritoryQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "UserTerritory Not Exist!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting UserTerritory!");
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
        [Route("GetZonesByUserInTerritory")]
        public async Task<ActionResult<List<GetZone>>> GetZonesByUserInTerritory(Guid UserId)
        {
            try
            {
                return await this.mediator.Send(new GetZonesByUserInTerritoryQuery(UserId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

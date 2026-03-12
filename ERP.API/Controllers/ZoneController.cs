using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Zone.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ZoneController : ControllerBase
    {
        private readonly IMediator mediator;

        public ZoneController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetZoneById")]
        public async Task<ActionResult<GetZone>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetZoneByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllZone")]
        public async Task<ActionResult<Tuple<IEnumerable<GetZone>, long>>> GetAll(GetAllZoneQuery getAllZoneQuery)
        {
            try
            {
                return await this.mediator.Send(getAllZoneQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveZone")]
        public async Task<IActionResult> Save(SaveZoneCommand command)
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
                        return this.Result(ResponseStatus.OK, "Zone Saved!", null);
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
        [Route("GetZoneByName")]
        public async Task<ActionResult<List<GetZone>>> GetZoneByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetZoneByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteZone")]
        public async Task<ActionResult<long>> DeleteZone(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteZoneQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Zone is used in Area!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Zone!");
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
        [Route("GetFieldMapFilter")]
        public async Task<ActionResult<GetFieldMapFilterEF>> GetFieldMapFilter(GetFieldMapFilterEFQuery getFieldMapFilterQuery)
        {
            try
            {
                return await this.mediator.Send(getFieldMapFilterQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetZoneByRegionId")]
        public async Task<ActionResult<List<GetZone>>> GetZoneByRegionId(long regionId)
        {
            try
            {
                return await this.mediator.Send(new GetZoneByRegionIdQuery(regionId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

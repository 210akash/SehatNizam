using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.IPD.Bed.Query;
using ERP.Mediator.Mediator.IPD.Bed.Command;
using Microsoft.AspNetCore.Authorization;
namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BedController : ControllerBase
    {
        private readonly IMediator mediator;

        public BedController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllBeds")]
        public async Task<ActionResult<Tuple<IEnumerable<GetBed>, long>>> GetAll(GetAllBedQuery getAllBedQuery)
        {
            try
            {
                return await this.mediator.Send(getAllBedQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveBed")]
        public async Task<IActionResult> Save(SaveBedCommand command)
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
                        return this.Result(ResponseStatus.OK, "Bed Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Bed No Already Exists!", null);
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

        [HttpDelete]
        [Route("DeleteBed")]
        public async Task<ActionResult<bool>> DeleteBed(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteBedQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBedByRoom")]
        public async Task<ActionResult<List<GetBed>>> GetBedByRoom(long RoomId)
        {
            try
            {
                return await mediator.Send(new GetBedByRoomQuery(RoomId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBedCode")]
        public async Task<ActionResult<string>> GetBedCode(long RoomId, long Id)
        {
            try
            {
                string code =  await mediator.Send(new GetBedCodeQuery(RoomId, Id));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

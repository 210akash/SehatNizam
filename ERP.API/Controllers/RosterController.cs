using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Roster.Query;
using ERP.Mediator.Mediator.Roster.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RosterController : ControllerBase
    {
        private readonly IMediator mediator;

        public RosterController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllRosters")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRoster>, long>>> GetAll(GetAllRosterQuery getAllRosterQuery)
        {
            try
            {
                return await this.mediator.Send(getAllRosterQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllRostersByManager")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRoster>, long>>> GetAll(GetAllRosterByManagerQuery getAllRosterByManagerQuery)
        {
            try
            {
                return await this.mediator.Send(getAllRosterByManagerQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveRoster")]
        public async Task<IActionResult> Save(SaveRosterCommand command)
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
                        return this.Result(ResponseStatus.OK, "Roster Saved!", null);
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

        [HttpPost]
        [Route("SaveRosterByManager")]
        public async Task<IActionResult> Save(SaveRosterByManagerCommand command)
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
                        return this.Result(ResponseStatus.OK, "Roster Saved!", null);
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

        [HttpDelete]
        [Route("DeleteRoster")]
        public async Task<ActionResult<bool>> DeleteRoster(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteRosterQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessRoster")]
        public async Task<ActionResult<bool>> ProcessRoster(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessRosterQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveRoster")]
        public async Task<ActionResult<bool>> ApproveRoster(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveRosterQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("RejectRoster")]
        public async Task<ActionResult<bool>> RejectRoster(long id)
        {
            try
            {
                return await this.mediator.Send(new RejectRosterQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetRosterCount")]
        public async Task<ActionResult<Tuple<long, long, long>>> GetRosterCount(GetRosterCountQuery getLeadsCountByUserProjectQuery)
        {
            try
            {
                return await this.mediator.Send(getLeadsCountByUserProjectQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

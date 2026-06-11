using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Issue.Command;
using ERP.Mediator.Mediator.BloodBank.Issue.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers.BloodBank
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BloodIssueController : ControllerBase
    {
        private readonly IMediator mediator;

        public BloodIssueController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllBloodIssues")]
        public async Task<ActionResult<Tuple<IEnumerable<GetBloodIssue>, long>>> GetAll(GetAllBloodIssueQuery query)
        {
            try
            {
                return await mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetBloodIssueWorklist")]
        public async Task<ActionResult<Tuple<IEnumerable<GetBloodIssueWorklist>, long>>> GetWorklist(GetBloodIssueWorklistQuery query)
        {
            try
            {
                return await mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveBloodIssue")]
        public async Task<IActionResult> Save(SaveBloodIssueCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Blood Issue Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, "Invalid blood issue data!", null);
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Conflict, "This request is not ready for issue!", null);
                }

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteBloodIssue")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await mediator.Send(new DeleteBloodIssueQuery(id));
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, true, "Successfully Deleted!");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Blood issue not found!");
                }

                return this.Result(ResponseStatus.Error, null, "There is some error!");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

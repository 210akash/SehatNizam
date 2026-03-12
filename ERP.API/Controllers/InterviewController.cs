using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Interview.Query;
using ERP.Mediator.Mediator.Interview.Command;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.IGP.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InterviewController : ControllerBase
    {
        private readonly IMediator mediator;

        public InterviewController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetInterviewById")]
        public async Task<ActionResult<GetInterview>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetInterviewByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllInterviews")]
        public async Task<ActionResult<Tuple<IEnumerable<GetInterview>, long>>> GetAll(GetAllInterviewQuery getAllInterviewQuery)
        {
            try
            {
                return await this.mediator.Send(getAllInterviewQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveInterview")]
        public async Task<IActionResult> Save(SaveInterviewCommand command)
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
                        return this.Result(ResponseStatus.OK, "Interview Saved!", null);
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
        [Route("AddComments")]
        public async Task<IActionResult> AddComments(AddCommentsCommand command)
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
                        return this.Result(ResponseStatus.OK, "Comments Saved!", null);
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
        [Route("GetInterviewByName")]
        public async Task<ActionResult<List<GetInterview>>> GetInterviewByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetInterviewByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteInterview")]
        public async Task<ActionResult<bool>> DeleteInterview(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteInterviewQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetInterviewAttendees")]
        public async Task<ActionResult<List<GetAllUsers>>> GetInterviewAttendees()
        {
            try
            {
                return await mediator.Send(new GetInterviewAttendeesQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCode")]
        public async Task<ActionResult<string>> GetCode()
        {
            try
            {
                string code = await mediator.Send(new GetCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

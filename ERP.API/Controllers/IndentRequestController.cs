using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Mediator.Mediator.IndentRequest.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndentRequestController : ControllerBase
    {
        private readonly IMediator mediator;

        public IndentRequestController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetIndentRequestById")]
        public async Task<ActionResult<GetIndentRequest>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetIndentRequestByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllIndentRequests")]
        public async Task<ActionResult<Tuple<IEnumerable<GetIndentRequest>, long>>> GetAll(GetAllIndentRequestQuery getAllIndentRequestQuery)
        {
            try
            {
                return await this.mediator.Send(getAllIndentRequestQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveIndentRequest")]
        public async Task<IActionResult> Save(SaveIndentRequestCommand command)
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
                        return this.Result(ResponseStatus.OK, "IndentRequest Saved!", null);
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
        [Route("DeleteIndentRequest")]
        public async Task<ActionResult<bool>> DeleteIndentRequest(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteIndentRequestQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetIndentRequestByCompany")]
        public async Task<ActionResult<List<GetIndentRequest>>> GetIndentRequestByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetIndentRequestByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetIndentRequestCode")]
        public async Task<ActionResult<string>> GetIndentRequestCode()
        {
            try
            {
                string code =  await mediator.Send(new GetIndentRequestCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessIndentRequest")]
        public async Task<ActionResult<bool>> ProcessIndentRequest(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessIndentRequestQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveIndentRequest")]
        public async Task<ActionResult<bool>> ApproveIndentRequest(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveIndentRequestQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetIndentRequestCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetIndentRequestCount(GetIndentRequestCountQuery getLeadsCountByUserProjectQuery)
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

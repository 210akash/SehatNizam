using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Mediator.Mediator.Issuance.Command;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.GRN.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IssuanceController : ControllerBase
    {
        private readonly IMediator mediator;

        public IssuanceController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetIssuanceById")]
        public async Task<ActionResult<GetIssuance>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetIssuanceByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllIssuances")]
        public async Task<ActionResult<Tuple<IEnumerable<GetIssuance>, long>>> GetAll(GetAllIssuanceQuery getAllIssuanceQuery)
        {
            try
            {
                return await this.mediator.Send(getAllIssuanceQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveIssuance")]
        public async Task<IActionResult> Save(SaveIssuanceCommand command)
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
                        return this.Result(ResponseStatus.OK, "Issuance Saved!", null);
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
        [Route("DeleteIssuance")]
        public async Task<ActionResult<bool>> DeleteIssuance(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteIssuanceQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetIssuanceByCompany")]
        public async Task<ActionResult<List<GetIssuance>>> GetIssuanceByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetIssuanceByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetIssuanceCode")]
        public async Task<ActionResult<string>> GetIssuanceCode()
        {
            try
            {
                string code =  await mediator.Send(new GetIssuanceCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessIssuance")]
        public async Task<ActionResult<bool>> ProcessIssuance(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessIssuanceQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveIssuance")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveIssuance(long id, string StatusRemarks)
        {
            try
            {
                return await this.mediator.Send(new ApproveIssuanceQuery(id, StatusRemarks));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetIssuanceCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetIssuanceCount(GetIssuanceCountQuery getLeadsCountByUserProjectQuery)
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

        [HttpGet]
        [Route("GetPendingIndentRequest")]
        public async Task<ActionResult<List<GetIndentRequest>>> GetPendingIndentRequest(long IndentRequestId)
        {
            try
            {
                return await mediator.Send(new GetPendingIndentRequestQuery(IndentRequestId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingIndentRequestItems")]
        public async Task<ActionResult<List<GetIndentRequestDetail>>> GetPendingIndentRequestItems(long IndentRequestId, long IssuanceId)
        {
            try
            {
                return await mediator.Send(new GetPendingIndentRequestItemsQuery(IndentRequestId, IssuanceId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

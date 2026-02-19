using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Mediator.Mediator.ComparativeStatement.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComparativeStatementController : ControllerBase
    {
        private readonly IMediator mediator;

        public ComparativeStatementController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetComparativeStatementById")]
        public async Task<ActionResult<GetComparativeStatement>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetComparativeStatementByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllComparativeStatements")]
        public async Task<ActionResult<Tuple<IEnumerable<GetComparativeStatement>, long>>> GetAll(GetAllComparativeStatementQuery getAllComparativeStatementQuery)
        {
            try
            {
                return await this.mediator.Send(getAllComparativeStatementQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveComparativeStatement")]
        public async Task<IActionResult> Save(SaveComparativeStatementCommand command)
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
                        return this.Result(ResponseStatus.OK, "ComparativeStatement Saved!", null);
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
        [Route("DeleteComparativeStatement")]
        public async Task<ActionResult<bool>> DeleteComparativeStatement(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteComparativeStatementQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetComparativeStatementByCompany")]
        public async Task<ActionResult<List<GetComparativeStatement>>> GetComparativeStatementByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetComparativeStatementByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetComparativeStatementCode")]
        public async Task<ActionResult<string>> GetComparativeStatementCode()
        {
            try
            {
                string code =  await mediator.Send(new GetComparativeStatementCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessComparativeStatement")]
        public async Task<ActionResult<bool>> ProcessComparativeStatement(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessComparativeStatementQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveComparativeStatement")]
        public async Task<ActionResult<bool>> ApproveComparativeStatement(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveComparativeStatementQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetComparativeStatementCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetComparativeStatementCount(GetComparativeStatementCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("getPendingDemand")]
        public async Task<ActionResult<List<GetDropDown>>> getPendingDemand(long PurchaseDemandId, long ComparativeStatementId)
        {
            try
            {
                return await mediator.Send(new GetPendingDemandQuery(PurchaseDemandId, ComparativeStatementId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingDemandItems")]
        public async Task<ActionResult<List<GetPurchaseDemandDetail>>> GetPendingDemandItems(long PurchaseDemandId, long ComparativeStatementId)
        {
            try
            {
                return await mediator.Send(new GetPendingDemandItemsQuery(PurchaseDemandId, ComparativeStatementId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

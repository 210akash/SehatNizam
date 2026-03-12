using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.CostSheet.Query;
using ERP.Mediator.Mediator.CostSheet.Command;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Mediator.Mediator.Company.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CostSheetController : ControllerBase
    {
        private readonly IMediator mediator;

        public CostSheetController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllCostSheets")]
        public async Task<ActionResult<Tuple<IEnumerable<GetCostSheet>, long>>> GetAll(GetAllCostSheetQuery getAllCostSheetQuery)
        {
            try
            {
                return await this.mediator.Send(getAllCostSheetQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveCostSheet")]
        public async Task<IActionResult> Save(SaveCostSheetCommand command)
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
                        return this.Result(ResponseStatus.OK, "CostSheet Saved!", null);
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
        [Route("DeleteCostSheet")]
        public async Task<ActionResult<bool>> DeleteCostSheet(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteCostSheetQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessCostSheet")]
        public async Task<ActionResult<bool>> ProcessCostSheet(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessCostSheetQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveCostSheet")]
        public async Task<ActionResult<bool>> ApproveCostSheet(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveCostSheetQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("RejectCostSheet")]
        public async Task<ActionResult<bool>> RejectCostSheet(long id)
        {
            try
            {
                return await this.mediator.Send(new RejectCostSheetQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCostSheetCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetCostSheetCount(GetCostSheetCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetCostSheetByItem")]
        public async Task<ActionResult<List<GetDropDown>>> GetCostSheetByItem(long ItemId)
        {
            try
            {
                return await mediator.Send(new GetCostSheetByItemQuery(ItemId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

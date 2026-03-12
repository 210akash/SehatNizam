using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.WarehouseTransfer.Query;
using ERP.Mediator.Mediator.WarehouseTransfer.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehouseTransferController : ControllerBase
    {
        private readonly IMediator mediator;

        public WarehouseTransferController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllWarehouseTransfers")]
        public async Task<ActionResult<Tuple<IEnumerable<GetWarehouseTransfer>, long>>> GetAll(GetAllWarehouseTransferQuery getAllWarehouseTransferQuery)
        {
            try
            {
                return await this.mediator.Send(getAllWarehouseTransferQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveWarehouseTransfer")]
        public async Task<IActionResult> Save(SaveWarehouseTransferCommand command)
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
                        return this.Result(ResponseStatus.OK, "WarehouseTransfer Saved!", null);
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
        [Route("DeleteWarehouseTransfer")]
        public async Task<ActionResult<bool>> DeleteWarehouseTransfer(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteWarehouseTransferQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessWarehouseTransfer")]
        public async Task<ActionResult<bool>> ProcessWarehouseTransfer(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessWarehouseTransferQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveWarehouseTransfer")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveWarehouseTransfer(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveWarehouseTransferQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetWarehouseTransferCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetWarehouseTransferCount(GetWarehouseTransferCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetWarehouseTransferCode")]
        public async Task<ActionResult<string>> GetWarehouseTransferCode()
        {
            try
            {
                string code = await mediator.Send(new GetWarehouseTransferCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("RevokeWarehouseTransfer")]
        public async Task<ActionResult<Tuple<long, string>>> RevokeWarehouseTransfer(long id)
        {
            try
            {
                return await this.mediator.Send(new RevokeWarehouseTransferQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingCostSheet")]
        public async Task<ActionResult<List<GetCostSheet>>> GetPendingCostSheetByItem(long ItemId, long ProjectId, long CostSheetId)
        {
            try
            {
                return await mediator.Send(new GetPendingCostSheetByItemQuery(ItemId, ProjectId, CostSheetId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

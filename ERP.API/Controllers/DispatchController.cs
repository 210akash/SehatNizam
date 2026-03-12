using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Mediator.Mediator.Dispatch.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DispatchController : ControllerBase
    {
        private readonly IMediator mediator;

        public DispatchController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        
        [HttpGet]
        [Route("GetDispatchById")]
        public async Task<ActionResult<GetDispatch>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetDispatchByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllDispatchs")]
        public async Task<ActionResult<Tuple<IEnumerable<GetDispatch>, long>>> GetAll(GetAllDispatchQuery getAllDispatchQuery)
        {
            try
            {
                return await this.mediator.Send(getAllDispatchQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveDispatch")]
        public async Task<IActionResult> Save(SaveDispatchCommand command)
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
                        return this.Result(ResponseStatus.OK, "Dispatch Saved!", null);
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
        [Route("DeleteDispatch")]
        public async Task<ActionResult<bool>> DeleteDispatch(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteDispatchQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDispatchByCompany")]
        public async Task<ActionResult<List<GetDispatch>>> GetDispatchByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetDispatchByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDispatchCode")]
        public async Task<ActionResult<string>> GetDispatchCode()
        {
            try
            {
                string code = await mediator.Send(new GetDispatchCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessDispatch")]
        public async Task<ActionResult<bool>> ProcessDispatch(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessDispatchQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveDispatch")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveDispatch(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveDispatchQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetDispatchCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetDispatchCount(GetDispatchCountQuery getLeadsCountByUserProjectQuery)
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

        [HttpPost]
        [Route("getPendingOrder")]
        public async Task<ActionResult<List<GetOrder>>> getPendingDemand(GetPendingOrderQuery OrderIds)
        {
            try
            {
                return await mediator.Send(OrderIds);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingOrderItems")]
        public async Task<ActionResult<List<GetOrderItems>>> GetPendingDemandItems(long OrderId, long DispatchId)
        {
            try
            {
                return await mediator.Send(new GetPendingOrderItemsQuery(OrderId, DispatchId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDispatchByOrderId")]
        public async Task<ActionResult<List<GetDispatchOrder>>> GetDispatchByOrderId(long orderId)
        {
            try
            {
                return await this.mediator.Send(new GetDispatchOrderByOrderIdQuery(orderId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ReceiveDispatchOrder")]
        public async Task<ActionResult<bool>> ReceiveDispatchOrder(long dispatchOrderId)
        {
            try
            {
                return await this.mediator.Send(new ReceiveDispatchOrderQuery(dispatchOrderId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateDispatchPrintStatus")]
        public async Task<IActionResult> UpdateDispatchPrintStatus(UpdateDispatchPrintStatusCommand command)
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
                        return this.Result(ResponseStatus.OK, "Print Status Update!", null);
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
        [Route("GetOrdersToDispatch")]
        public async Task<ActionResult<Tuple<IEnumerable<GetOrder>, long>>> GetOrdersToDispatch(GetOrdersToDispatchQuery getOrdersToDispatchQuery)
        {
            try
            {
                return await mediator.Send(getOrdersToDispatchQuery);
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

        [HttpGet]
        [Route("RejectDispatch")]
        public async Task<ActionResult<Tuple<long, string>>> RejectDispatch(long id)
        {
            try
            {
                return await this.mediator.Send(new RejectDispatchQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

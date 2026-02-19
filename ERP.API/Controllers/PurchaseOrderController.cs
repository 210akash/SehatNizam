using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Mediator.Mediator.PurchaseOrder.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IMediator mediator;

        public PurchaseOrderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetPurchaseOrderById")]
        public async Task<ActionResult<GetPurchaseOrder>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetPurchaseOrderByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPurchaseOrders")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPurchaseOrder>, long>>> GetAll(GetAllPurchaseOrderQuery getAllPurchaseOrderQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPurchaseOrderQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePurchaseOrder")]
        public async Task<IActionResult> Save(SavePurchaseOrderCommand command)
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
                        return this.Result(ResponseStatus.OK, "PurchaseOrder Saved!", null);
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
        [Route("DeletePurchaseOrder")]
        public async Task<ActionResult<bool>> DeletePurchaseOrder(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePurchaseOrderQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPurchaseOrderByCompany")]
        public async Task<ActionResult<List<GetPurchaseOrder>>> GetPurchaseOrderByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetPurchaseOrderByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPurchaseOrderCode")]
        public async Task<ActionResult<string>> GetPurchaseOrderCode()
        {
            try
            {
                string code =  await mediator.Send(new GetPurchaseOrderCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessPurchaseOrder")]
        public async Task<ActionResult<bool>> ProcessPurchaseOrder(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessPurchaseOrderQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApprovePurchaseOrder")]
        public async Task<ActionResult<bool>> ApprovePurchaseOrder(long id, string StatusRemarks)
        {
            try
            {
                return await this.mediator.Send(new ApprovePurchaseOrderQuery(id, StatusRemarks));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPurchaseOrderCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetPurchaseOrderCount(GetPurchaseOrderCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingDemand")]
        public async Task<ActionResult<List<GetDropDown>>> GetPendingDemand(long PurchaseDemandId)
        {
            try
            {
                return await mediator.Send(new GetPendingDemandQuery(PurchaseDemandId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingDemandItems")]
        public async Task<ActionResult<List<GetPurchaseDemandDetail>>> GetPendingDemandItems(long PurchaseDemandId, long PurchaseOrderId,long VendorId)
        {
            try
            {
                return await mediator.Send(new GetPendingDemandItemsQuery(PurchaseDemandId, PurchaseOrderId, VendorId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

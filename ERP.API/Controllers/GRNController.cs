using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Mediator.Mediator.GRN.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GRNController : ControllerBase
    {
        private readonly IMediator mediator;

        public GRNController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetGRNById")]
        public async Task<ActionResult<GetGRN>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetGRNByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllGRNs")]
        public async Task<ActionResult<Tuple<IEnumerable<GetGRN>, long>>> GetAll(GetAllGRNQuery getAllGRNQuery)
        {
            try
            {
                return await this.mediator.Send(getAllGRNQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveGRN")]
        public async Task<IActionResult> Save(SaveGRNCommand command)
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
                        return this.Result(ResponseStatus.OK, "GRN Saved!", null);
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
        [Route("DeleteGRN")]
        public async Task<ActionResult<bool>> DeleteGRN(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteGRNQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetGRNCode")]
        public async Task<ActionResult<string>> GetGRNCode()
        {
            try
            {
                string code = await mediator.Send(new GetGRNCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessGRN")]
        public async Task<ActionResult<bool>> ProcessGRN(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessGRNQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveGRN")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveGRN(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveGRNQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetGRNCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetGRNCount(GetGRNCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingInspection")]
        public async Task<ActionResult<List<GetInspection>>> GetPendingInspection(long InspectionId)
        {
            try
            {
                return await mediator.Send(new GetPendingInspectionsQuery(InspectionId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingInspectionItems")]
        public async Task<ActionResult<List<GetInspectionDetail>>> GetPendingInspectionItems(long InspectionId,long GRNId)
        {
            try
            {
                return await mediator.Send(new GetPendingInspectionsItemsQuery(InspectionId, GRNId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPurchaseInvoices")]
        public async Task<ActionResult<Tuple<IEnumerable<GetGRN>, long>>> GetAllPurchaseInvoices(GetAllPurchaseInvoiceQuery getAllPurchaseInvoiceQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPurchaseInvoiceQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPurchaseInvoiceCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetPurchaseInvoiceCount(GetPurchaseInvoiceCountQuery getPurchaseInvoiceCountQuery)
        {
            try
            {
                return await this.mediator.Send(getPurchaseInvoiceCountQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateWHTPercentage")]
        public async Task<IActionResult> UpdateWHTPercentage(SavePurchaseInvoiceWHTCommand command)
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
                        return this.Result(ResponseStatus.OK, "WHT Percentage Updated!", null);
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

        [HttpGet]
        [Route("ApprovePurchaseInvoice")]
        public async Task<ActionResult<Tuple<long, string>>> ApprovePurchaseInvoice(long id)
        {
            try
            {
                return await this.mediator.Send(new ApprovePurchaseInvoiceQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingCostSheet")]
        public async Task<ActionResult<List<GetDropDown>>> GetPendingCostSheetQuery(long ItemId, long CostSheetId = 0)
        {
            try
            {
                return await mediator.Send(new GetPendingCostSheetQuery(ItemId, CostSheetId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessPurchaseInvoice")]
        public async Task<ActionResult<long>> ProcessPurchaseInvoice(long id, string comments)
        {
            try
            {
                return await this.mediator.Send(new ProcessPurchaseInvoiceQuery(id, comments));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("RejectPurchaseInvoice")]
        public async Task<ActionResult<long>> RejectPurchaseInvoice(long id, string comments)
        {
            try
            {
                return await this.mediator.Send(new RejectPurchaseInvoiceQuery(id, comments));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

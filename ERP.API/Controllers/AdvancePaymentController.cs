using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AdvancePayments.Command;
using ERP.Mediator.Mediator.AdvancePayments.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvancePaymentsController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdvancePaymentsController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
        [Route("GetAllAdvancePayments")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAdvancePayment>, long>>> GetAll(GetAllAdvancePaymentsQuery getAllAdvancePaymentsQuery)
        {
            try
            {
                return await this.mediator.Send(getAllAdvancePaymentsQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAdvancePayment")]
        public async Task<IActionResult> Save(SaveAdvancePaymentsCommand command)
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
                        return this.Result(ResponseStatus.OK, "AdvancePayments Saved!", null);
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
        [Route("DeleteAdvancePayment")]
        public async Task<ActionResult<bool>> DeleteAdvancePayments(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteAdvancePaymentsQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("ConfirmAdvancePayment")]
        public async Task<ActionResult<bool>> ConfirmAdvancePayments(long id)
        {
            try
            {
                return await this.mediator.Send(new ConfirmAdvancePaymentCommand(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

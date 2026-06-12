using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AppointmentPayments.Command;
using ERP.Mediator.Mediator.AppointmentPayments.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentPaymentController : ControllerBase
    {
        private readonly IMediator mediator;

        public AppointmentPaymentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAppointmentPaymentGroups")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointmentPaymentGroup>, long>>> GetGroups(GetAppointmentPaymentGroupsQuery query)
        {
            try
            {
                return await this.mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllAppointmentPayments")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointmentPayment>, long>>> GetAll(GetAllAppointmentPaymentsQuery query)
        {
            try
            {
                return await this.mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAppointmentPayment")]
        public async Task<IActionResult> Save(SaveAppointmentPaymentCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Appointment Payment Updated!", null);
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, "Payment record not found!", null);
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Conflict, "Approved payment cannot be edited!", null);
                }
                else if (result == 422)
                {
                    return this.Result(ResponseStatus.Error, "Discount cannot be greater than visit fee!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, "There is some error!", null);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ApproveAppointmentPayments")]
        public async Task<IActionResult> Approve(ApproveAppointmentPaymentsCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Payments approved successfully!", null);
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, "No pending payments found for this appointment!", null);
                }
                else if (result == 422)
                {
                    return this.Result(ResponseStatus.Error, "Discount must be between 0 and visit fee!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, "No payments selected for approval!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, "There is some error!", null);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.PaymentMode.Query;
using ERP.Mediator.Mediator.PaymentMode.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentModeController : ControllerBase
    {
        private readonly IMediator mediator;

        public PaymentModeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetPaymentModeById")]
        public async Task<ActionResult<GetPaymentMode>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetPaymentModeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPaymentModes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPaymentMode>, long>>> GetAll(GetAllPaymentModeQuery getAllPaymentModeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPaymentModeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePaymentMode")]
        public async Task<IActionResult> Save(SavePaymentModeCommand command)
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
                        return this.Result(ResponseStatus.OK, "PaymentMode Saved!", null);
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
        [Route("GetPaymentModeByName")]
        public async Task<ActionResult<List<GetPaymentMode>>> GetPaymentModeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetPaymentModeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeletePaymentMode")]
        public async Task<ActionResult<bool>> DeletePaymentMode(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePaymentModeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPaymentModeByCompany")]
        public async Task<ActionResult<List<GetPaymentMode>>> GetPaymentModeByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetPaymentModeByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

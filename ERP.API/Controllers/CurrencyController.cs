using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Currency.Query;
using ERP.Mediator.Mediator.Currency.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IMediator mediator;

        public CurrencyController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetCurrencyById")]
        public async Task<ActionResult<GetCurrency>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetCurrencyByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllCurrencys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetCurrency>, long>>> GetAll(GetAllCurrencyQuery getAllCurrencyQuery)
        {
            try
            {
                return await this.mediator.Send(getAllCurrencyQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveCurrency")]
        public async Task<IActionResult> Save(SaveCurrencyCommand command)
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
                        return this.Result(ResponseStatus.OK, "Currency Saved!", null);
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
        [Route("GetCurrencyByName")]
        public async Task<ActionResult<List<GetCurrency>>> GetCurrencyByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetCurrencyByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCurrency")]
        public async Task<ActionResult<bool>> DeleteCurrency(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteCurrencyQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCurrencyByCompany")]
        public async Task<ActionResult<List<GetCurrency>>> GetCurrencyByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetCurrencyByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

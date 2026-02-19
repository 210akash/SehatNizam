using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.DeliveryTerms.Query;
using ERP.Mediator.Mediator.DeliveryTerms.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryTermsController : ControllerBase
    {
        private readonly IMediator mediator;

        public DeliveryTermsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetDeliveryTermsById")]
        public async Task<ActionResult<GetDeliveryTerms>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetDeliveryTermsByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllDeliveryTerms")]
        public async Task<ActionResult<Tuple<IEnumerable<GetDeliveryTerms>, long>>> GetAll(GetAllDeliveryTermsQuery getAllDeliveryTermsQuery)
        {
            try
            {
                return await this.mediator.Send(getAllDeliveryTermsQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveDeliveryTerms")]
        public async Task<IActionResult> Save(SaveDeliveryTermsCommand command)
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
                        return this.Result(ResponseStatus.OK, "DeliveryTerms Saved!", null);
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
        [Route("GetDeliveryTermsByName")]
        public async Task<ActionResult<List<GetDeliveryTerms>>> GetDeliveryTermsByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetDeliveryTermsByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteDeliveryTerms")]
        public async Task<ActionResult<bool>> DeleteDeliveryTerms(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteDeliveryTermsQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDeliveryTermsByCompany")]
        public async Task<ActionResult<List<GetDeliveryTerms>>> GetDeliveryTermsByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetDeliveryTermsByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Priority.Query;
using ERP.Mediator.Mediator.Priority.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriorityController : ControllerBase
    {
        private readonly IMediator mediator;

        public PriorityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetPriorityById")]
        public async Task<ActionResult<GetPriority>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetPriorityByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPrioritys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPriority>, long>>> GetAll(GetAllPriorityQuery getAllPriorityQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPriorityQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePriority")]
        public async Task<IActionResult> Save(SavePriorityCommand command)
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
                        return this.Result(ResponseStatus.OK, "Priority Saved!", null);
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
        [Route("GetPriorityByName")]
        public async Task<ActionResult<List<GetPriority>>> GetPriorityByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetPriorityByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeletePriority")]
        public async Task<ActionResult<bool>> DeletePriority(long id)
        {
            try
            {
                return await this.mediator.Send(new DeletePriorityQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPriorityByCompany")]
        public async Task<ActionResult<List<GetPriority>>> GetPriorityByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetPriorityByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

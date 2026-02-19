using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.IndentType.Query;
using ERP.Mediator.Mediator.IndentType.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndentTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public IndentTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetIndentTypeById")]
        public async Task<ActionResult<GetIndentType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetIndentTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllIndentTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetIndentType>, long>>> GetAll(GetAllIndentTypeQuery getAllIndentTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllIndentTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveIndentType")]
        public async Task<IActionResult> Save(SaveIndentTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "IndentType Saved!", null);
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
        [Route("GetIndentTypeByName")]
        public async Task<ActionResult<List<GetIndentType>>> GetIndentTypeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetIndentTypeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteIndentType")]
        public async Task<ActionResult<bool>> DeleteIndentType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteIndentTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetIndentTypeByCompany")]
        public async Task<ActionResult<List<GetIndentType>>> GetIndentTypeByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetIndentTypeByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

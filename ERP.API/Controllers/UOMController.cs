using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.UOM.Query;
using ERP.Mediator.Mediator.UOM.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UOMController : ControllerBase
    {
        private readonly IMediator mediator;

        public UOMController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetUOMById")]
        public async Task<ActionResult<GetUOM>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetUOMByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllUOMs")]
        public async Task<ActionResult<Tuple<IEnumerable<GetUOM>, long>>> GetAll(GetAllUOMQuery getAllUOMQuery)
        {
            try
            {
                return await this.mediator.Send(getAllUOMQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveUOM")]
        public async Task<IActionResult> Save(SaveUOMCommand command)
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
                        return this.Result(ResponseStatus.OK, "UOM Saved!", null);
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
        [Route("GetUOMByName")]
        public async Task<ActionResult<List<GetUOM>>> GetUOMByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetUOMByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteUOM")]
        public async Task<ActionResult<bool>> DeleteUOM(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteUOMQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUOMByCompany")]
        public async Task<ActionResult<List<GetUOM>>> GetUOMByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetUOMByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

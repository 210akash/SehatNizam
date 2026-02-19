using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.GST.Query;
using ERP.Mediator.Mediator.GST.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GSTController : ControllerBase
    {
        private readonly IMediator mediator;

        public GSTController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetGSTById")]
        public async Task<ActionResult<GetGST>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetGSTByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllGST")]
        public async Task<ActionResult<Tuple<IEnumerable<GetGST>, long>>> GetAll(GetAllGSTQuery getAllGSTQuery)
        {
            try
            {
                return await this.mediator.Send(getAllGSTQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveGST")]
        public async Task<IActionResult> Save(SaveGSTCommand command)
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
                        return this.Result(ResponseStatus.OK, "GST Saved!", null);
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
        [Route("GetGSTByName")]
        public async Task<ActionResult<List<GetGST>>> GetGSTByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetGSTByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteGST")]
        public async Task<ActionResult<bool>> DeleteGST(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteGSTQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetGSTByCompany")]
        public async Task<ActionResult<List<GetGST>>> GetGSTByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetGSTByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCurrentGST")]
        public async Task<ActionResult<GetGST>> GetCurrentGST()
        {
            try
            {
                return await this.mediator.Send(new GetCurrentGSTQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

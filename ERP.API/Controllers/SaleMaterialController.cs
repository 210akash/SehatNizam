using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.SaleMaterial.Query;
using ERP.Mediator.Mediator.SaleMaterial.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleMaterialController : ControllerBase
    {
        private readonly IMediator mediator;

        public SaleMaterialController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllSaleMaterials")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSaleMaterial>, long>>> GetAll(GetAllSaleMaterialQuery getAllSaleMaterialQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSaleMaterialQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSaleMaterial")]
        public async Task<IActionResult> Save(SaveSaleMaterialCommand command)
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
                        return this.Result(ResponseStatus.OK, "SaleMaterial Saved!", null);
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
        [Route("DeleteSaleMaterial")]
        public async Task<ActionResult<bool>> DeleteSaleMaterial(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteSaleMaterialQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessSaleMaterial")]
        public async Task<ActionResult<bool>> ProcessSaleMaterial(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessSaleMaterialQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveSaleMaterial")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveSaleMaterial(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveSaleMaterialQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("RejectSaleMaterial")]
        public async Task<ActionResult<bool>> RejectSaleMaterial(long id)
        {
            try
            {
                return await this.mediator.Send(new RejectSaleMaterialQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSaleMaterialCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetSaleMaterialCount(GetSaleMaterialCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetSaleMaterialCode")]
        public async Task<ActionResult<string>> GetSaleMaterialCode()
        {
            try
            {
                string code = await mediator.Send(new GetSaleMaterialCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

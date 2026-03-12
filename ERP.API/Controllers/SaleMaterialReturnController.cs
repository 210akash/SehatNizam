using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Mediator.Mediator.SaleMaterialReturn.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleMaterialReturnController : ControllerBase
    {
        private readonly IMediator mediator;

        public SaleMaterialReturnController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetSaleMaterialReturnById")]
        public async Task<ActionResult<GetSaleMaterialReturn>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetSaleMaterialReturnByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllSaleMaterialReturns")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSaleMaterialReturn>, long>>> GetAll(GetAllSaleMaterialReturnQuery getAllSaleMaterialReturnQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSaleMaterialReturnQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSaleMaterialReturn")]
        public async Task<IActionResult> Save(SaveSaleMaterialReturnCommand command)
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
                        return this.Result(ResponseStatus.OK, "Sale Material Return Saved!", null);
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
        [Route("DeleteSaleMaterialReturn")]
        public async Task<ActionResult<bool>> DeleteSaleMaterialReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteSaleMaterialReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSaleMaterialReturnCode")]
        public async Task<ActionResult<string>> GetSaleMaterialReturnCode()
        {
            try
            {
                string code = await mediator.Send(new GetSaleMaterialReturnCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessSaleMaterialReturn")]
        public async Task<ActionResult<bool>> ProcessSaleMaterialReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessSaleMaterialReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveSaleMaterialReturn")]
        public async Task<ActionResult<Tuple<long, string>>> ApproveSaleMaterialReturn(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveSaleMaterialReturnQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSaleMaterialReturnCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetSaleMaterialReturnCount(GetSaleMaterialReturnCountQuery getLeadsCountByUserProjectQuery)
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

        [HttpPost]
        [Route("GetPendingSaleMaterial")]
        public async Task<ActionResult<List<GetSaleMaterial>>> GetPendingSaleMaterial(GetPendingSaleMaterialQuery getPendingSaleMaterialQuery)
        {
            try
            {
                return await mediator.Send(getPendingSaleMaterialQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingSaleMaterialItems")]
        public async Task<ActionResult<List<GetSaleMaterialDetail>>> GetPendingSaleMaterialItems(long SaleMaterialId, long SaleMaterialReturnId)
        {
            try
            {
                return await mediator.Send(new GetPendingSaleMaterialItemsQuery(SaleMaterialId, SaleMaterialReturnId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

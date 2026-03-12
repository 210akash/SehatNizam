using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Mediator.Mediator.Inspection.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InspectionController : ControllerBase
    {
        private readonly IMediator mediator;

        public InspectionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetInspectionById")]
        public async Task<ActionResult<GetInspection>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetInspectionByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllInspections")]
        public async Task<ActionResult<Tuple<IEnumerable<GetInspection>, long>>> GetAll(GetAllInspectionQuery getAllInspectionQuery)
        {
            try
            {
                return await this.mediator.Send(getAllInspectionQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveInspection")]
        public async Task<IActionResult> Save(SaveInspectionCommand command)
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
                        return this.Result(ResponseStatus.OK, "Inspection Saved!", null);
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
        [Route("DeleteInspection")]
        public async Task<ActionResult<bool>> DeleteInspection(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteInspectionQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetInspectionCode")]
        public async Task<ActionResult<string>> GetInspectionCode()
        {
            try
            {
                string code = await mediator.Send(new GetInspectionCodeQuery());
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessInspection")]
        public async Task<ActionResult<bool>> ProcessInspection(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessInspectionQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


        [HttpGet]
        [Route("ApproveInspection")]
        public async Task<ActionResult<bool>> ApproveInspection(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveInspectionQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetInspectionCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetInspectionCount(GetInspectionCountQuery getLeadsCountByUserProjectQuery)
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
        [Route("GetPendingIGPs")]
        public async Task<ActionResult<List<GetDropDown>>> GetPendingIGPs(long IGPId)
        {
            try
            {
                return await mediator.Send(new GetPendingIGPsQuery(IGPId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingIGPItems")]
        public async Task<ActionResult<List<GetIGPDetails>>> GetPendingIGPItems(long IGPId, long InspectionId)
        {
            try
            {
                return await mediator.Send(new GetPendingIGPItemsQuery(IGPId, InspectionId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

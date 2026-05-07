using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.LabOrderType.Command;
using ERP.Mediator.Mediator.LabOrderType.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabOrderTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public LabOrderTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetLabOrderTypeById")]
        public async Task<ActionResult<GetLabOrderType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetLabOrderTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllLabOrderTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetLabOrderType>, long>>> GetAll(GetAllLabOrderTypeQuery query)
        {
            try
            {
                return await this.mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveLabOrderType")]
        public async Task<IActionResult> Save(SaveLabOrderTypeCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Lab Order Type Saved!", null);
                }

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteLabOrderType")]
        public async Task<ActionResult<bool>> DeleteLabOrderType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteLabOrderTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

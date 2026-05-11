using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.RadiologyType.Command;
using ERP.Mediator.Mediator.RadiologyType.Query;
using MediatR;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RadiologyTypeController : BaseController
    {
        private readonly IMediator mediator;

        public RadiologyTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllRadiologyTypes")]
        public async Task<ActionResult<IEnumerable<GetRadiologyType>>> GetAllRadiologyTypes([FromBody] GetAllRadiologyTypesQuery query)
        {
            try
            {
                var result = await this.mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
     
        [HttpPost]
        [Route("SaveRadiologyType")]
        public async Task<ActionResult<int>> SaveRadiologyType([FromBody] SaveRadiologyTypeCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Radiology Type Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data! Name is required.");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Radiology Type not found!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Error, null, "Radiology Type with this name already exists!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Radiology Type!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteRadiologyType")]
        public async Task<ActionResult<bool>> DeleteRadiologyType(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteRadiologyTypeCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Radiology Type Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Radiology Type not found!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

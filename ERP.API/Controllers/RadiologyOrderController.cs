using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.RadiologyOrder.Command;
using ERP.Mediator.Mediator.RadiologyOrder.Query;
using MediatR;
using ERP.Mediator.Mediator.RadiologyOrder.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RadiologyOrderController : BaseController
    {
        private readonly IMediator mediator;

        public RadiologyOrderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetRadiologyOrderById")]
        public async Task<ActionResult<GetRadiologyOrder>> GetRadiologyOrderById(long id)
        {
            try
            {
                var result = await this.mediator.Send(new GetRadiologyOrderByIdQuery(id));
                if (result == null)
                {
                    return this.Result(ResponseStatus.Error, null, "Radiology Order not found!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllRadiologyOrders")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRadiologyOrder>, long>>> GetAll(GetAllRadiologyOrderQuery query) => await mediator.Send(query);

        [HttpPost]
        [Route("SaveRadiologyOrder")]
        public async Task<ActionResult<int>> SaveRadiologyOrder([FromBody] SaveRadiologyOrderCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);

                if (result > 0)
                {
                    return this.Result(ResponseStatus.OK, result, "Radiology Order Saved!");
                }
                else if (result == -400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data! Please fill all required fields.");
                }
                else if (result == -404)
                {
                    return this.Result(ResponseStatus.Error, null, "Radiology Order not found!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Radiology Order!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteRadiologyOrder")]
        public async Task<ActionResult<bool>> DeleteRadiologyOrder(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteRadiologyOrderCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Radiology Order Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Radiology Order not found!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveRadiologyResult")]
        public async Task<IActionResult> SaveRadiologyResult([FromBody] SaveRadiologyStudyResultCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);

                if (result > 0)
                    return this.Result(ResponseStatus.OK, result, "Radiology Result Saved!");

                return this.Result(ResponseStatus.Error, null, "Unable to save radiology result!");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ConfirmRadiologyOrder")]
        public async Task<IActionResult> ConfirmRadiologyOrder([FromBody] ConfirmRadiologyOrderCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);

                if (result)
                    return this.Result(ResponseStatus.OK, result, "Radiology Order Confirmed!");

                return this.Result(ResponseStatus.Error, null, "Unable to confirm radiology order!");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

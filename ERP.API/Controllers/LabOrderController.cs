using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.LabOrder.Command;
using ERP.Mediator.Mediator.LabOrder.Query;
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
    public class LabOrderController : ControllerBase
    {
        private readonly IMediator mediator;
        public LabOrderController(IMediator mediator) { this.mediator = mediator; }

        [HttpGet]
        [Route("GetLabOrderById")]
        public async Task<ActionResult<GetLabOrder>> GetById(long id) => await mediator.Send(new GetLabOrderByIdQuery(id));

        [HttpPost]
        [Route("GetAllLabOrders")]
        public async Task<ActionResult<Tuple<IEnumerable<GetLabOrder>, long>>> GetAll(GetAllLabOrderQuery query) => await mediator.Send(query);

        [HttpPost]
        [Route("SaveLabOrder")]
        public async Task<IActionResult> Save(SaveLabOrderCommand command)
        {
            try
            {
                var result = await mediator.Send(command);

                if (result > 0)
                    return this.Result(ResponseStatus.OK, result, "Lab Order Saved!");

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteLabOrder")]
        public async Task<ActionResult<bool>> Delete(long id) => await mediator.Send(new DeleteLabOrderQuery(id));

        [HttpPost]
        [Route("SaveLabResult")]
        public async Task<IActionResult> SaveLabResult(SaveLabResultCommand command)
        {
            try
            {
                var result = await mediator.Send(command);

                if (result > 0)
                    return this.Result(ResponseStatus.OK, result, "Lab Result Saved!");

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ConfirmLabOrder")]
        public async Task<IActionResult> ConfirmLabOrder(ConfirmLabOrderCommand command)
        {
            try
            {
                var result = await mediator.Send(command);

                if (result)
                    return this.Result(ResponseStatus.OK, result, "Lab Result Saved!");

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

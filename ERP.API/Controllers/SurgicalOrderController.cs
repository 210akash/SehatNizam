using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SurgicalOrder.Command;
using ERP.Mediator.Mediator.SurgicalOrder.Query;
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
    public class SurgicalOrderController : ControllerBase
    {
        private readonly IMediator mediator;

        public SurgicalOrderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllSurgicalOrders")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSurgicalOrder>, long>>> GetAll(GetAllSurgicalOrdersQuery query)
        {
            try
            {
                return await mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSurgicalOrder")]
        public async Task<IActionResult> Save(SaveSurgicalOrderCommand command)
        {
            try
            {
                var result = await mediator.Send(command);
                if (result > 0)
                    return this.Result(ResponseStatus.OK, result, "Surgical Order Saved!");

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteSurgicalOrder")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                var result = await mediator.Send(new DeleteSurgicalOrderQuery(id));
                if (result)
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");

                return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Vehicle.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Vehicle.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class VehicleController : ControllerBase
    {
        private readonly IMediator mediator;

        public VehicleController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetVehicleById")]
        public async Task<ActionResult<GetVehicle>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetVehicleByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllVehicle")]
        public async Task<ActionResult<Tuple<IEnumerable<GetVehicle>, long>>> GetAll(GetAllVehicleQuery getAllVehicleQuery)
        {
            try
            {
                return await this.mediator.Send(getAllVehicleQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveVehicle")]
        public async Task<IActionResult> Save(SaveVehicleCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shop Type Saved!", null);
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
        [Route("DeleteVehicle")]
        public async Task<ActionResult<long>> DeleteVehicle(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteVehicleQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Shop Type is used in Shop!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Vehicle!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetVehiclesByDealership")]
        public async Task<ActionResult<List<GetVehicle>>> GetVehiclesByDealership(long dealershipId)
        {
            try
            {
                return await this.mediator.Send(new GetVehiclesByDealershipQuery(dealershipId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetVehicleByName")]
        public async Task<ActionResult<List<GetVehicle>>> GetVehicleByName(string name)
        {
            try
            {
                return await this.mediator.Send(new GetVehicleByNameQuery(name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

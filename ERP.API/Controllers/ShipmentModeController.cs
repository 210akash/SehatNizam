using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.ShipmentMode.Query;
using ERP.Mediator.Mediator.ShipmentMode.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentModeController : ControllerBase
    {
        private readonly IMediator mediator;

        public ShipmentModeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetShipmentModeById")]
        public async Task<ActionResult<GetShipmentMode>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetShipmentModeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllShipmentModes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetShipmentMode>, long>>> GetAll(GetAllShipmentModeQuery getAllShipmentModeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllShipmentModeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveShipmentMode")]
        public async Task<IActionResult> Save(SaveShipmentModeCommand command)
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
                        return this.Result(ResponseStatus.OK, "ShipmentMode Saved!", null);
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
        [Route("GetShipmentModeByName")]
        public async Task<ActionResult<List<GetShipmentMode>>> GetShipmentModeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetShipmentModeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteShipmentMode")]
        public async Task<ActionResult<bool>> DeleteShipmentMode(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteShipmentModeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShipmentModeByCompany")]
        public async Task<ActionResult<List<GetShipmentMode>>> GetShipmentModeByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetShipmentModeByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

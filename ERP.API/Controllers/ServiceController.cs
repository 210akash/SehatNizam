using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.API.Helper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Service.Command;
using ERP.Mediator.Mediator.Service.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IMediator mediator;

        public ServiceController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllServices")]
        public async Task<ActionResult<IEnumerable<GetService>>> GetAllServices([FromBody] GetAllServicesQuery query)
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

        [HttpGet]
        [Route("GetServiceById")]
        public async Task<ActionResult<GetService>> GetServiceById(long id)
        {
            try
            {
                var result = await this.mediator.Send(new GetServiceByIdQuery(id));
                if (result == null)
                {
                    return this.Result(ResponseStatus.Error, null, "Service not found!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveService")]
        public async Task<ActionResult<int>> SaveService([FromBody] SaveServiceCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Service Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data! Code and Name are required, BasePrice must be non-negative.");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Service not found!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Error, null, "Service with this code already exists!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Service!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteService")]
        public async Task<ActionResult<bool>> DeleteService(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteServiceCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Service Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Service not found!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

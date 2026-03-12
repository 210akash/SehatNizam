using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeDevice.Command;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.Company.Query;
using ERP.Mediator.Mediator.City.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeDeviceController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeDeviceController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("SaveEmployeeDevice")]
        public async Task<ActionResult<Tuple<long, string>>> Save(SaveEmployeeDeviceCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    return await this.mediator.Send(command);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetEmployeeDevice")]
        public async Task<ActionResult<List<GetEmployeeDevice>>> GetById(GetDevicesByEmployeeQuery getDevicesByEmployeeQuery)
        {
            try
            {
                return await this.mediator.Send(getDevicesByEmployeeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

    }
}

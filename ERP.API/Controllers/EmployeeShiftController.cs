using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeShift.Query;
using ERP.Mediator.Mediator.EmployeeShift.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeShiftController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeShiftController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeShiftById")]
        public async Task<ActionResult<GetEmployeeShift>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeShiftByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeShifts")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeShift>, long>>> GetAll(GetAllEmployeeShiftQuery getAllEmployeeShiftQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeShiftQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeShift")]
        public async Task<IActionResult> Save(SaveEmployeeShiftCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Shift Saved!", null);
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
        [Route("GetEmployeeShiftByName")]
        public async Task<ActionResult<List<GetEmployeeShift>>> GetEmployeeShiftByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeShiftByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeShift")]
        public async Task<ActionResult<bool>> DeleteEmployeeShift(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeShiftQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

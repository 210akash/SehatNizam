using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeLeaveType.Query;
using ERP.Mediator.Mediator.EmployeeLeaveType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeLeaveTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeLeaveTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeLeaveTypeById")]
        public async Task<ActionResult<GetEmployeeLeaveType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeLeaveTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeLeaveTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeLeaveType>, long>>> GetAll(GetAllEmployeeLeaveTypeQuery getAllEmployeeLeaveTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeLeaveTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeLeaveType")]
        public async Task<IActionResult> Save(SaveEmployeeLeaveTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Leave Type Saved!", null);
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
        [Route("GetEmployeeLeaveTypeByName")]
        public async Task<ActionResult<List<GetEmployeeLeaveType>>> GetEmployeeLeaveTypeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeLeaveTypeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeLeaveType")]
        public async Task<ActionResult<bool>> DeleteEmployeeLeaveType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeLeaveTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

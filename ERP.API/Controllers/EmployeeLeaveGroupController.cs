using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Query;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeLeaveGroupController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeLeaveGroupController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeLeaveGroupById")]
        public async Task<ActionResult<GetEmployeeLeaveGroup>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeLeaveGroupByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeLeaveGroups")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeLeaveGroup>, long>>> GetAll(GetAllEmployeeLeaveGroupQuery getAllEmployeeLeaveGroupQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeLeaveGroupQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeLeaveGroup")]
        public async Task<IActionResult> Save(SaveEmployeeLeaveGroupCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Leave Group Saved!", null);
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
        [Route("GetEmployeeLeaveGroupByName")]
        public async Task<ActionResult<List<GetEmployeeLeaveGroup>>> GetEmployeeLeaveGroupByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeLeaveGroupByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeLeaveGroup")]
        public async Task<ActionResult<bool>> DeleteEmployeeLeaveGroup(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeLeaveGroupQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveGroupLeaveType")]
        public async Task<IActionResult> SaveGroupLeaveType(SaveEmployeeGroupLeaveTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "Group Leave Type Saved!", null);
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


    }
}

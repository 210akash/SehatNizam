using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Mediator.Mediator.EmployeeLeave.Command;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.Account.Query;
using static ERP.API.Controllers.AccountController;
using System.Linq;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeLeaveController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeLeaveController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeLeaveById")]
        public async Task<ActionResult<GetEmployeeLeave>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeLeaveByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeLeaves")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeLeave>, long>>> GetAll(GetAllEmployeeLeaveQuery getAllEmployeeLeaveQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeLeaveQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllDepartmentLeaves")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeLeave>, long>>> GetAll(GetAllDepartmentLeaveQuery getAllEmployeeLeaveQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeLeaveQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        

        [HttpPost]
        [Route("SaveEmployeeLeave")]
        public async Task<IActionResult> Save(SaveEmployeeLeaveCommand command)
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
                    if (result == "Success")
                    {
                        return this.Result(ResponseStatus.OK, "Employee Leave Saved!", null);
                    }
                    else if (result.Contains("Conflict"))
                    {
                        return this.Result(ResponseStatus.Conflict, result, null);
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

        [HttpPost]
        [Route("SaveEmployeeLeaveByHr")]
        public async Task<IActionResult> SaveEmployeeLeaveByHr(SaveEmployeeLeaveByHrCommand command)
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
                    if (result == "Success")
                    {
                        return this.Result(ResponseStatus.OK, "Employee Leave Saved!", null);
                    }
                    else if(result.Contains("Conflict"))
                    {
                        return this.Result(ResponseStatus.Conflict, result, null);
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
        [Route("GetEmployeeLeaveByName")]
        public async Task<ActionResult<List<GetEmployeeLeave>>> GetEmployeeLeaveByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeLeaveByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeLeave")]
        public async Task<ActionResult<bool>> DeleteEmployeeLeave(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeLeaveQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEmployeeLeaveBalance")]
        public async Task<ActionResult<List<LeaveBalanceDto>>> GetEmployeeLeaveBalance()
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeLeaveBalanceQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetLeaveBalanceByEmployee")]
        public async Task<ActionResult<List<LeaveBalanceDto>>> GetLeaveBalanceByEmployee(Guid EmployeeId)
        {
            try
            {
                return await this.mediator.Send(new GetLeaveBalanceByEmployeeQuery(EmployeeId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ProcessEmployeeLeave")]
        public async Task<ActionResult<long>> ProcessEmployeeLeave(ProcessEmployeeLeaveCommand processEmployeeLeaveCommand)
        {
            try
            {
                return await this.mediator.Send(processEmployeeLeaveCommand);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ApproveEmployeeLeave")]
        public async Task<ActionResult<long>> ApproveEmployeeLeave(ApproveEmployeeLeaveCommand approveEmployeeLeaveCommand)
        {
            try
            {
                return await this.mediator.Send(approveEmployeeLeaveCommand);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("RejectEmployeeLeave")]
        public async Task<ActionResult<long>> RejectEmployeeLeave(RejectEmployeeLeaveCommand rejectEmployeeLeaveCommand)
        {
            try
            {
                return await this.mediator.Send(rejectEmployeeLeaveCommand);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSingleEmployeeLeaves")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeLeave>, long>>> GetSingleEmployeeLeaves(GetSingleEmployeeLeaveQuery getSingleEmployeeLeaveQuery)
        {
            try
            {
                return await this.mediator.Send(getSingleEmployeeLeaveQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ManagerApproveLeave")]
        public async Task<ActionResult<long>> ManagerApproveLeave(ManagerApproveLeaveCommand managerApproveLeaveCommand)
        {
            try
            {
                return await this.mediator.Send(managerApproveLeaveCommand);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

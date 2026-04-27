using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command;
namespace ERP.API.Controllers.EmployeeSalary
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSalaryController : BaseController
    {
        private readonly IMediator mediator;

        public EmployeeSalaryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        #region EmployeeSalary Endpoints

        [HttpPost]
        [Route("GetAllEmployeeSalaries")]
        public async Task<ActionResult<IEnumerable<GetEmployeeSalary>>> GetAllEmployeeSalaries([FromBody] GetAllEmployeeSalariesQuery query)
        {
            try
            {
                var result = await mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEmployeeSalaryByEmployeeId")]
        public async Task<ActionResult<IEnumerable<GetEmployeeSalary>>> GetEmployeeSalaryByEmployeeId(string employeeId)
        {
            try
            {
                var result = await mediator.Send(new GetEmployeeSalaryByEmployeeIdQuery
                {
                    EmployeeId = employeeId
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetLatestEmployeeSalaries")]
        public async Task<ActionResult<IEnumerable<GetEmployeeSalary>>> GetLatestEmployeeSalaries(string employeeId, DateTime asOfDate)
        {
            try
            {
                var result = await mediator.Send(new GetLatestEmployeeSalariesQuery
                {
                    EmployeeId = employeeId,
                    AsOfDate = asOfDate
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeSalary")]
        public async Task<ActionResult<int>> SaveEmployeeSalary([FromBody] SaveEmployeeSalaryCommand command)
        {
            try
            {
                var result = await mediator.Send(command);
                if (result.Item1 == 200)
                {
                    return this.Result(ResponseStatus.OK,  null, "Employee Salary Saved!");
                }
                else if (result.Item1 == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data!");
                }
                else if (result.Item1 == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Employee Salary not found!");
                }
                else if (result.Item1 == 409)
                {
                    return this.Result(ResponseStatus.Error, null, result.Item2);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Employee Salary!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeSalary")]
        public async Task<ActionResult<bool>> DeleteEmployeeSalary(long id)
        {
            try
            {
                var result = await mediator.Send(new DeleteEmployeeSalaryCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Employee Salary Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Employee Salary not found!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        #endregion

    }
}

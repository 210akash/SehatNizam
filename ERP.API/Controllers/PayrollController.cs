using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query;
using ERP.Mediator.Mediator.Payroll.Payroll.Command;
using ERP.Mediator.Mediator.Payroll.Payroll.Query;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Command;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : BaseController
    {
        private readonly IMediator mediator;

        public PayrollController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        #region SalaryHead Endpoints

        [HttpGet]
        [Route("GetAllSalaryHeads")]
        public async Task<ActionResult<IEnumerable<GetSalaryHead>>> GetAllSalaryHeads()
        {
            try
            {
                var result = await this.mediator.Send(new GetAllSalaryHeadsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSalaryHeadById")]
        public async Task<ActionResult<GetSalaryHead>> GetSalaryHeadById(long id)
        {
            try
            {
                var result = await this.mediator.Send(new GetSalaryHeadByIdQuery(id));
                if (result == null)
                {
                    return this.Result(ResponseStatus.Error, null, "Salary Head not found!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSalaryHead")]
        public async Task<ActionResult<int>> SaveSalaryHead([FromBody] SaveSalaryHeadCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Salary Head Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data!");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Salary Head not found!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Error, null, "Salary Head with this name already exists!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Salary Head!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteSalaryHead")]
        public async Task<ActionResult<bool>> DeleteSalaryHead(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteSalaryHeadCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Salary Head Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Salary Head not found!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        #endregion

        #region EmployeeSalary Endpoints

        [HttpPost]
        [Route("GetAllEmployeeSalaries")]
        public async Task<ActionResult<IEnumerable<GetEmployeeSalary>>> GetAllEmployeeSalaries([FromBody] GetAllEmployeeSalariesQuery query)
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
        [Route("GetLatestEmployeeSalaries")]
        public async Task<ActionResult<IEnumerable<GetEmployeeSalary>>> GetLatestEmployeeSalaries(long employeeId, DateTime asOfDate)
        {
            try
            {
                var result = await this.mediator.Send(new GetLatestEmployeeSalariesQuery
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
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Employee Salary Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data!");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Employee Salary not found!");
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
                var result = await this.mediator.Send(new DeleteEmployeeSalaryCommand(id));
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

        #region Payroll Endpoints

        [HttpPost]
        [Route("GetAllPayrolls")]
        public async Task<ActionResult<IEnumerable<GetPayroll>>> GetAllPayrolls([FromBody] GetAllPayrollsQuery query)
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
        [Route("GetPayrollById")]
        public async Task<ActionResult<GetPayroll>> GetPayrollById(long id)
        {
            try
            {
                var result = await this.mediator.Send(new GetPayrollByIdQuery(id));
                if (result == null)
                {
                    return this.Result(ResponseStatus.Error, null, "Payroll not found!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePayroll")]
        public async Task<ActionResult<int>> SavePayroll([FromBody] SavePayrollCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Payroll Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data!");
                }
                else if (result == 403)
                {
                    return this.Result(ResponseStatus.Error, null, "Cannot modify paid payroll!");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Payroll not found!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Error, null, "Payroll already exists for this month/year!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Payroll!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GeneratePayroll")]
        public async Task<ActionResult<int>> GeneratePayroll([FromBody] GeneratePayrollCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Payroll Generated Successfully!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid month/year!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Error, null, "Payroll already approved/paid for this month/year!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error generating Payroll!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeletePayroll")]
        public async Task<ActionResult<bool>> DeletePayroll(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeletePayrollCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Payroll Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Payroll not found or cannot delete approved/paid payroll!");
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

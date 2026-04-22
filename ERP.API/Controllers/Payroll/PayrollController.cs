using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.Mediator.Mediator.Payroll.Payroll.Query;
using ERP.Mediator.Mediator.Payroll.Payroll.Command;

namespace ERP.API.Controllers.Payroll
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

        #region Payroll Endpoints

        [HttpPost]
        [Route("GetAllPayrolls")]
        public async Task<ActionResult<IEnumerable<GetPayroll>>> GetAllPayrolls([FromBody] GetAllPayrollsQuery query)
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
        [Route("GetPayrollById")]
        public async Task<ActionResult<GetPayroll>> GetPayrollById(long id)
        {
            try
            {
                var result = await mediator.Send(new GetPayrollByIdQuery(id));
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
                var result = await mediator.Send(command);
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
                var result = await mediator.Send(command);
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
                var result = await mediator.Send(new DeletePayrollCommand(id));
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

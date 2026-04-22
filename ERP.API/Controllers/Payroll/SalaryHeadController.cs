using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Query;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Command;
using ERP.Mediator.Mediator.AccountFlow.Query;

namespace ERP.API.Controllers.SalaryHead
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryHeadController : BaseController
    {
        private readonly IMediator mediator;

        public SalaryHeadController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        #region SalaryHead Endpoints

        [HttpPost]
        [Route("GetAllSalaryHeads")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSalaryHead>, long>>> GetAll(GetAllSalaryHeadsQuery getAllSalaryHeadsQuery)
        {
            try
            {
                var result = await mediator.Send(getAllSalaryHeadsQuery);
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
                var result = await mediator.Send(command);
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
                var result = await mediator.Send(new DeleteSalaryHeadCommand(id));
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

    }
}

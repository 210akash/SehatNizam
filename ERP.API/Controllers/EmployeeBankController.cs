using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeBank.Query;
using ERP.Mediator.Mediator.EmployeeBank.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeBankController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeBankController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeBankById")]
        public async Task<ActionResult<GetEmployeeBank>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeBankByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeBanks")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeBank>, long>>> GetAll(GetAllEmployeeBankQuery getAllEmployeeBankQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeBankQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeBank")]
        public async Task<IActionResult> Save(SaveEmployeeBankCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Bank Saved!", null);
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
        [Route("GetEmployeeBankByName")]
        public async Task<ActionResult<List<GetEmployeeBank>>> GetEmployeeBankByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeBankByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeBank")]
        public async Task<ActionResult<bool>> DeleteEmployeeBank(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeBankQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

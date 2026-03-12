using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeType.Query;
using ERP.Mediator.Mediator.EmployeeType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeTypeById")]
        public async Task<ActionResult<GetEmployeeType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeType>, long>>> GetAll(GetAllEmployeeTypeQuery getAllEmployeeTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeType")]
        public async Task<IActionResult> Save(SaveEmployeeTypeCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Type Saved!", null);
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
        [Route("GetEmployeeTypeByName")]
        public async Task<ActionResult<List<GetEmployeeType>>> GetEmployeeTypeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeTypeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeType")]
        public async Task<ActionResult<bool>> DeleteEmployeeType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

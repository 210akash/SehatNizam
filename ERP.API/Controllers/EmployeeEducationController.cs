using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeEducation.Query;
using ERP.Mediator.Mediator.EmployeeEducation.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeEducationController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeEducationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeEducationById")]
        public async Task<ActionResult<GetEmployeeEducation>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeEducationByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeEducations")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeEducation>, long>>> GetAll(GetAllEmployeeEducationQuery getAllEmployeeEducationQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeEducationQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeEducation")]
        public async Task<IActionResult> Save(SaveEmployeeEducationCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Education Saved!", null);
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
        [Route("GetEmployeeEducationByName")]
        public async Task<ActionResult<List<GetEmployeeEducation>>> GetEmployeeEducationByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeEducationByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeEducation")]
        public async Task<ActionResult<bool>> DeleteEmployeeEducation(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeEducationQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

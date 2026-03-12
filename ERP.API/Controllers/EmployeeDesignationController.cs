using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeDesignation.Query;
using ERP.Mediator.Mediator.EmployeeDesignation.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeDesignationController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeDesignationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeDesignationById")]
        public async Task<ActionResult<GetEmployeeDesignation>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeDesignationByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeDesignations")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeDesignation>, long>>> GetAll(GetAllEmployeeDesignationQuery getAllEmployeeDesignationQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeDesignationQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeDesignation")]
        public async Task<IActionResult> Save(SaveEmployeeDesignationCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Designation Saved!", null);
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
        [Route("GetEmployeeDesignationByName")]
        public async Task<ActionResult<List<GetEmployeeDesignation>>> GetEmployeeDesignationByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeDesignationByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeDesignation")]
        public async Task<ActionResult<bool>> DeleteEmployeeDesignation(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeDesignationQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

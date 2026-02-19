using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Department.Query;
using ERP.Mediator.Mediator.Department.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator mediator;

        public DepartmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetDepartmentById")]
        public async Task<ActionResult<GetDepartment>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetDepartmentByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllDepartments")]
        public async Task<ActionResult<Tuple<IEnumerable<GetDepartment>, long>>> GetAll(GetAllDepartmentQuery getAllDepartmentQuery)
        {
            try
            {
                return await this.mediator.Send(getAllDepartmentQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveDepartment")]
        public async Task<IActionResult> Save(SaveDepartmentCommand command)
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
                        return this.Result(ResponseStatus.OK, "Department Saved!", null);
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
        [Route("GetDepartmentByName")]
        public async Task<ActionResult<List<GetDepartment>>> GetDepartmentByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetDepartmentByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteDepartment")]
        public async Task<ActionResult<bool>> DeleteDepartment(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteDepartmentQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDepartmentByCompany")]
        public async Task<ActionResult<List<GetDepartment>>> GetDepartmentByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetDepartmentByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

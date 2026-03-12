using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeGrade.Query;
using ERP.Mediator.Mediator.EmployeeGrade.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeGradeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeGradeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeGradeById")]
        public async Task<ActionResult<GetEmployeeGrade>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeGradeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeGrades")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeGrade>, long>>> GetAll(GetAllEmployeeGradeQuery getAllEmployeeGradeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeGradeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeGrade")]
        public async Task<IActionResult> Save(SaveEmployeeGradeCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Grade Saved!", null);
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
        [Route("GetEmployeeGradeByName")]
        public async Task<ActionResult<List<GetEmployeeGrade>>> GetEmployeeGradeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeGradeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeGrade")]
        public async Task<ActionResult<bool>> DeleteEmployeeGrade(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeGradeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}

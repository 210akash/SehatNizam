using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeDocumentType.Query;
using ERP.Mediator.Mediator.EmployeeDocumentType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeDocumentTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeDocumentTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetEmployeeDocumentTypeById")]
        public async Task<ActionResult<GetEmployeeDocumentType>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeDocumentTypeByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllEmployeeDocumentTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeDocumentType>, long>>> GetAll(GetAllEmployeeDocumentTypeQuery getAllEmployeeDocumentTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeDocumentTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeDocumentType")]
        public async Task<IActionResult> Save(SaveEmployeeDocumentTypeCommand command)
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
        [Route("GetEmployeeDocumentTypeByName")]
        public async Task<ActionResult<List<GetEmployeeDocumentType>>> GetEmployeeDocumentTypeByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetEmployeeDocumentTypeByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteEmployeeDocumentType")]
        public async Task<ActionResult<bool>> DeleteEmployeeDocumentType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeDocumentTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEmployeeDocumentByEmployeeId")]
        public async Task<ActionResult<List<GetEmployeeDocument>>> GetEmployeeDocumentByEmployeeId(Guid EmployeeId)
        {
            try
            {
                return await this.mediator.Send(new GetEmployeeDocumentByEmployeeIdQuery(EmployeeId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

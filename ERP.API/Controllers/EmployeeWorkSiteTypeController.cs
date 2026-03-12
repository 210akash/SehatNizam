using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.EmployeeWorkSiteType.Query;
using ERP.Mediator.Mediator.EmployeeWorkSiteType.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeWorkSiteTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeWorkSiteTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllEmployeeWorkSiteTypes")]
        public async Task<ActionResult<Tuple<IEnumerable<GetEmployeeWorkSiteType>, long>>> GetAll(GetAllEmployeeWorkSiteTypeQuery getAllEmployeeWorkSiteTypeQuery)
        {
            try
            {
                return await this.mediator.Send(getAllEmployeeWorkSiteTypeQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEmployeeWorkSiteType")]
        public async Task<IActionResult> Save(SaveEmployeeWorkSiteTypeCommand command)
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

        [HttpDelete]
        [Route("DeleteEmployeeWorkSiteType")]
        public async Task<ActionResult<bool>> DeleteEmployeeWorkSiteType(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteEmployeeWorkSiteTypeQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

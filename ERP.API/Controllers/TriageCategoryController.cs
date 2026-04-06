using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.TriageCategory.Query;
using ERP.Mediator.Mediator.TriageCategory.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TriageCategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public TriageCategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllTriageCategorys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetTriageCategory>, long>>> GetAll(GetAllTriageCategoryQuery getAllTriageCategoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllTriageCategoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveTriageCategory")]
        public async Task<IActionResult> Save(SaveTriageCategoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "TriageCategory Saved!", null);
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
        [Route("DeleteTriageCategory")]
        public async Task<ActionResult<bool>> DeleteTriageCategory(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteTriageCategoryQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

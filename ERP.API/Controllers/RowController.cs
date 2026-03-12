using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Row.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Row.Command;
using ERP.Mediator.Mediator.SubCategory.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RowController : ControllerBase
    {
        private readonly IMediator mediator;

        public RowController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpGet]
        [Route("GetRowByRackId")]
        public async Task<ActionResult<List<GetRow>>> GetRowByRackId(long Id)
        {
            try
            {
                return await mediator.Send(new GetRowByRackIdQuery(Id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
       
        [HttpPost]
        [Route("GetAllRow")]
        public async Task<ActionResult<Tuple<IEnumerable<GetRow>, long>>> GetAll(GetAllRowQuery getAllRowQuery)
        {
            try
            {
                return await this.mediator.Send(getAllRowQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveRow")]
        public async Task<IActionResult> Save(SaveRowCommand command)
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
                        return this.Result(ResponseStatus.OK, "Row Saved!", null);
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
        [Route("GetRowByName")]
        public async Task<ActionResult<List<GetRow>>> GetRowByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetRowByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteRow")]
        public async Task<ActionResult<long>> DeleteRow(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteRowQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Row is used in Zone!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Row!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

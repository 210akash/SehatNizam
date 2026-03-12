using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Section.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Section.Command;
using ERP.Mediator.Mediator.Row.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SectionController : ControllerBase
    {
        private readonly IMediator mediator;

        public SectionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetSectionByRowId")]
        public async Task<ActionResult<List<GetSection>>> GetSectionByRowId(long Id)
        {
            try
            {
                return await mediator.Send(new GetSectionByRowIdQuery(Id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        //[HttpGet]
        //[Route("GetSectionById")]
        //public async Task<ActionResult<GetSection>> GetById(long id)
        //{
        //    try
        //    {
        //        return await this.mediator.Send(new GetSectionByIdQuery(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("GetAllSection")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSection>, long>>> GetAll(GetAllSectionQuery getAllSectionQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSectionQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSection")]
        public async Task<IActionResult> Save(SaveSectionCommand command)
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
                        return this.Result(ResponseStatus.OK, "Section Saved!", null);
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
        [Route("GetSectionByName")]
        public async Task<ActionResult<List<GetSection>>> GetSectionByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetSectionByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteSection")]
        public async Task<ActionResult<long>> DeleteSection(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteSectionQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Section is used in Zone!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Section!");
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

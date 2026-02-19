using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Location.Query;
using ERP.Mediator.Mediator.Location.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IMediator mediator;

        public LocationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetLocationById")]
        public async Task<ActionResult<GetLocation>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetLocationByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllLocations")]
        public async Task<ActionResult<Tuple<IEnumerable<GetLocation>, long>>> GetAll(GetAllLocationQuery getAllLocationQuery)
        {
            try
            {
                return await this.mediator.Send(getAllLocationQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveLocation")]
        public async Task<IActionResult> Save(SaveLocationCommand command)
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
                        return this.Result(ResponseStatus.OK, "Location Saved!", null);
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
        [Route("GetLocationByName")]
        public async Task<ActionResult<List<GetLocation>>> GetLocationByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetLocationByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteLocation")]
        public async Task<ActionResult<bool>> DeleteLocation(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteLocationQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetLocationByCompany")]
        public async Task<ActionResult<List<GetLocation>>> GetLocationByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetLocationByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

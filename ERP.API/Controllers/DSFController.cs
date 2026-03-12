using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.DSF.Command;
using ERP.Mediator.Mediator.DSF.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DSFController : ControllerBase
    {
        private readonly IMediator mediator;

        public DSFController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAll")]
        public async Task<ActionResult<Tuple<IEnumerable<GetUsers>, long>>> GetAll(GetAllDSFQuery getAllDSFQuery)
        {
            try
            {
                return await this.mediator.Send(getAllDSFQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("AddDSFRoute")]
        public async Task<IActionResult> AddDSFRoute(AddDSFRouteCommand command)
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
                        return this.Result(ResponseStatus.OK, "Routes Added!", null);
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

        [HttpPost]
        [Route("GetAllSalesman")]
        public async Task<ActionResult<Tuple<IEnumerable<GetUsers>, long>>> GetAllSalesman(GetAllSalesmenQuery getAllSalesmenQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSalesmenQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Auth.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IMediator mediator;

        public DoctorController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllDoctors")]
        public async Task<ActionResult<Tuple<List<GetAllUsers>, long>>> GetAllDoctors(GetAllDoctorsQuery command)
        {
            try
            {
                return await this.mediator.Send(command);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

  
    }
}

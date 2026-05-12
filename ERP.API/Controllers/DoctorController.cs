using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Auth.Query;
using ERP.Mediator.Mediator.Doctor.Command;
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

        [HttpPost]
        [Route("SaveDoctorProfile")]
        public async Task<ActionResult<int>> SaveDoctorProfile([FromBody] SaveDoctorProfileCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Doctor Profile Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data! Please check the fields.");
                }
                else if (result == 404)
                {
                    return this.Result(ResponseStatus.Error, null, "Doctor Profile not found!");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Error, null, "Doctor Profile already exists!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Error saving Doctor Profile!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

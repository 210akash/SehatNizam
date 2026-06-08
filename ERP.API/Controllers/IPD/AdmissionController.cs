using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.Admission.Command;
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
    public class AdmissionController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdmissionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        //[HttpPost]
        //[Route("GetAllAdmissions")]
        //public async Task<ActionResult<Tuple<IEnumerable<GetAdmission>, long>>> GetAllAdmissions(GetAllAdmissionQuery command)
        //{
        //    try
        //    {
        //        return await this.mediator.Send(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("GetAllAdmissionByDoctor")]
        //public async Task<ActionResult<Tuple<IEnumerable<GetAdmission>, long>>> GetAllAdmissionByDoctor(GetAllAdmissionByDoctorQuery command)
        //{
        //    try
        //    {
        //        return await this.mediator.Send(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllAdmissionStatus")]
        //public async Task<ActionResult<List<GetAdmissionStatus>>> GetAllAdmissionStatus()
        //{
        //    try
        //    {
        //        return await this.mediator.Send(new GetAllAdmissionStatusQuery());
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAdmissionByToken")]
        //public async Task<ActionResult<List<GetAdmission>>> GetAdmissionByToken(string Token, long StatusId)
        //{
        //    try
        //    {
        //        return await this.mediator.Send(new GetAdmissionByTokenQuery(Token, StatusId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("SaveAdmission")]
        public async Task<IActionResult> Save(SaveAdmissionCommand command)
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
                        return this.Result(ResponseStatus.OK, "Admission Saved!", null);
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


        //[HttpPost]
        //[Route("ConfirmAdmission")]
        //public async Task<ActionResult<Tuple<long, string>>> ConfirmAdmission(ConfirmAppoinmentQuery confirmAppoinmentQuery)
        //{
        //    try
        //    {
        //        return await this.mediator.Send(confirmAppoinmentQuery);
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.Message);
        //    }
        //}
    }
}

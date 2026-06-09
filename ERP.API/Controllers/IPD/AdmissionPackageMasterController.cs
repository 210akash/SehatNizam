using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.AdmissionPackage.Command;
using ERP.Mediator.Mediator.IPD.AdmissionPackage.Query;
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
    public class AdmissionPackageMasterController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdmissionPackageMasterController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAdmissionPackages")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAdmissionPackageMaster>, long>>> GetAll(GetAllAdmissionPackageMasterQuery query)
        {
            try
            {
                return await mediator.Send(query);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAdmissionPackageById")]
        public async Task<ActionResult<GetAdmissionPackageMaster>> GetById(long id)
        {
            try
            {
                return await mediator.Send(new GetAdmissionPackageMasterByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAdmissionPackage")]
        public async Task<IActionResult> Save(SaveAdmissionPackageMasterCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await mediator.Send(command);
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Admission Package Saved!", null);
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, "Name and at least one service are required!", null);
                }

                return this.Result(ResponseStatus.Error, "There is some error!", null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAdmissionPackage")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                return await mediator.Send(new DeleteAdmissionPackageMasterQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

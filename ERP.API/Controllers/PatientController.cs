using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Patient.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IMediator mediator;

        public PatientController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllPatients")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPatient>, long>>> GetAll(GetAllPatientQuery getAllPatientQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPatientQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


        [HttpGet]
        [Route("GetPatientByName")]
        public async Task<ActionResult<List<GetPatient>>> GetPatientByName(string Search)
        {
            try
            {
                return await mediator.Send(new GetPatientByNameQuery(Search));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

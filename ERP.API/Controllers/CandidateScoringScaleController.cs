using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.CandidateScoringScale.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CandidateScoringScaleController : ControllerBase
    {
        private readonly IMediator mediator;

        public CandidateScoringScaleController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllCandidateScoringScales")]
        public async Task<ActionResult<IEnumerable<GetCandidateScoringScale>>> GetAll(GetAllCandidateScoringScaleQuery getAllCandidateScoringScaleQuery)
        {
            try
            {
                return await this.mediator.Send(getAllCandidateScoringScaleQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

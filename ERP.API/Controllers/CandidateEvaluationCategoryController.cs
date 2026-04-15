using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.CandidateEvaluationCategory.Query;
using ERP.Mediator.Mediator.CandidateEvaluationCategory.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidateEvaluationCategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public CandidateEvaluationCategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
        [Route("GetAllCandidateEvaluationCategorys")]
        public async Task<ActionResult<Tuple<IEnumerable<GetCandidateEvaluationCategory>, long>>> GetAll(GetAllCandidateEvaluationCategoryQuery getAllCandidateEvaluationCategoryQuery)
        {
            try
            {
                return await this.mediator.Send(getAllCandidateEvaluationCategoryQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveCandidateEvaluationCategory")]
        public async Task<IActionResult> Save(SaveCandidateEvaluationCategoryCommand command)
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
                        return this.Result(ResponseStatus.OK, "CandidateEvaluationCategory Saved!", null);
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

        [HttpDelete]
        [Route("DeleteCandidateEvaluationCategory")]
        public async Task<ActionResult<bool>> DeleteCandidateEvaluationCategory(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteCandidateEvaluationCategoryQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

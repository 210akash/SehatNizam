using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.AuditReview.Query;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.Account.Command;
using ERP.Mediator.Mediator.AuditReview.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditReviewController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuditReviewController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAuditReviews")]
        public async Task<ActionResult<Tuple<IEnumerable<GetOrder>, long>>> GetAllAuditReviews(GetOrdersOnlyByStatusQuery getOrdersByStatusQuery)
        {
            try
            {
                return await this.mediator.Send(getOrdersByStatusQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAuditReviewCount")]
        public async Task<ActionResult<Tuple<long, long, long,long>>> GetAuditReviewCount(GetOrdersCountByStatusQuery getOrdersCountByStatusQuery)
        {
            try
            {
                return await this.mediator.Send(getOrdersCountByStatusQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveAuditReview")]
        public async Task<IActionResult> SaveAuditReview(SaveAuditReviewCommand command)
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
                        return this.Result(ResponseStatus.OK, "Account Reviewed Saved!", null);
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

    }
}

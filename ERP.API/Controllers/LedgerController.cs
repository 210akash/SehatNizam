using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.Mediator.Mediator.Ledger.Query;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LedgerController : ControllerBase
    {
        private readonly IMediator mediator;

        public LedgerController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("CustomerCurrentBalance")]
        public async Task<ActionResult<decimal>> CustomerCurrentBalance(long CustomerId)
        {
            try
            {
                return await this.mediator.Send(new GetCustomerBalanceQuery(CustomerId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ItemCurrentBalance")]
        public async Task<ActionResult<decimal>> ItemCurrentBalance(long ItemId)
        {
            try
            {
                return await this.mediator.Send(new GetItemBalanceQuery(ItemId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

    }
}

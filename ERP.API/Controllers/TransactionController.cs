using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Mediator.Mediator.Transaction.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator mediator;

        public TransactionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetTransactionById")]
        public async Task<ActionResult<GetTransaction>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetTransactionByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllTransactions")]
        public async Task<ActionResult<Tuple<IEnumerable<GetTransaction>, long>>> GetAll(GetAllTransactionQuery getAllTransactionQuery)
        {
            try
            {
                return await this.mediator.Send(getAllTransactionQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveTransaction")]
        public async Task<IActionResult> Save(SaveTransactionCommand command)
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
                        return this.Result(ResponseStatus.OK, "Transaction Saved!", null);
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
        [Route("DeleteTransaction")]
        public async Task<ActionResult<bool>> DeleteTransaction(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteTransactionQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTransactionByCompany")]
        public async Task<ActionResult<List<GetTransaction>>> GetTransactionByCompany(long CompanyId)
        {
            try
            {
                return await mediator.Send(new GetTransactionByCompanyQuery(CompanyId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTransactionCode")]
        public async Task<ActionResult<string>> GetTransactionCode(long VoucherTypeId)
        {
            try
            {
                string code =  await mediator.Send(new GetTransactionCodeQuery(VoucherTypeId));
                return Ok(new { code = code });
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ProcessTransaction")]
        public async Task<ActionResult<bool>> ProcessTransaction(long id)
        {
            try
            {
                return await this.mediator.Send(new ProcessTransactionQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("ApproveTransaction")]
        public async Task<ActionResult<bool>> ApproveTransaction(long id)
        {
            try
            {
                return await this.mediator.Send(new ApproveTransactionQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetTransactionCount")]
        public async Task<ActionResult<Tuple<long, long, long, long>>> GetTransactionCount(GetTransactionCountQuery getLeadsCountByUserProjectQuery)
        {
            try
            {
                return await this.mediator.Send(getLeadsCountByUserProjectQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Mediator.Mediator.Employee.Query;
using ERP.Mediator.Mediator.AccountCategory.Query;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployeeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetEmployeeByName")]
        public async Task<ActionResult<List<GetEmployee>>> GetAccountByName(GetEmployeeByNameQuery request)
        {
            try
            {
                return await this.mediator.Send(request);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}

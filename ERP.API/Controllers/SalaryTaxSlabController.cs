using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.SalaryTaxSlab.Query;
using ERP.Mediator.Mediator.SalaryTaxSlab.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalaryTaxSlabController : ControllerBase
    {
        private readonly IMediator mediator;

        public SalaryTaxSlabController(IMediator mediator)
        {
            this.mediator = mediator;
        }
    
        [HttpPost]
        [Route("GetAllSalaryTaxSlab")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSalaryTaxSlab>, long>>> GetAll(GetAllSalaryTaxSlabQuery getAllSalaryTaxSlabQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSalaryTaxSlabQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSalaryTaxSlab")]
        public async Task<IActionResult> Save(SaveSalaryTaxSlabCommand command)
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
                        return this.Result(ResponseStatus.OK, "Employee Grade Saved!", null);
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
        [Route("DeleteSalaryTaxSlab")]
        public async Task<ActionResult<bool>> DeleteSalaryTaxSlab(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteSalaryTaxSlabQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        
    }
}
